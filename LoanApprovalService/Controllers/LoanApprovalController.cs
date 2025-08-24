using Bank.Shared.Events;
using LoanApprovalService.Entities.Enums;
using LoanApprovalService.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoanApprovalService.Interfaces;

namespace LoanApprovalService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoanApprovalController : ControllerBase
	{
		private readonly LoanApprovalDbContext _context;
		private readonly IRabbitMQPublisher _publisher;

		public LoanApprovalController(LoanApprovalDbContext context, IRabbitMQPublisher publisher)
		{
			_context = context;
			_publisher = publisher;
		}

		[HttpGet("GetAllApprovals")]
		public async Task<IActionResult> GetAllApprovals()
		{
			var approvals = await _context.LoanApprovals
				.Where(x => x.Status == LoanStatus.PendingApproval)
				.ToListAsync();
			if (approvals == null || !approvals.Any())
				return NotFound("No pending approvals found.");
			return Ok(approvals);
		}

		[HttpPost("approve")]
		public async Task<IActionResult> Approve([FromBody] ApproveRequest request)
		{
			var approval = await _context.LoanApprovals
				.FirstOrDefaultAsync(x => x.LoanId == request.LoanId && x.Status == LoanStatus.PendingApproval);

			if (approval == null) return NotFound();

			approval.Status = LoanStatus.Approved;
			approval.ApprovedAt = DateTime.UtcNow;
			approval.ApprovedById = request.ManagerId;

			await _context.SaveChangesAsync();

			_publisher.Publish(new LoanApprovedEvent(approval.LoanId, approval.AccountId, approval.Amount));
			return Ok(new { approval.LoanId, approval.Status });
		}

		[HttpPost("reject")]
		public async Task<IActionResult> Reject([FromBody] RejectRequest request)
		{
			var approval = await _context.LoanApprovals
				.FirstOrDefaultAsync(x => x.LoanId == request.LoanId && x.Status == LoanStatus.PendingApproval);

			if (approval == null) return NotFound();

			approval.Status = LoanStatus.Rejected;
			approval.RejectedAt = DateTime.UtcNow;
			approval.RejectedById = request.ManagerId;
			approval.RejectionReason = request.Reason;

			await _context.SaveChangesAsync();

			_publisher.Publish(new LoanRejectedEvent(approval.LoanId, approval.AccountId, request.Reason));
			return Ok(new { approval.LoanId, approval.Status });
		}
	}

	public record ApproveRequest(Guid LoanId, Guid ManagerId);
	public record RejectRequest(Guid LoanId, Guid ManagerId, string Reason);
}
