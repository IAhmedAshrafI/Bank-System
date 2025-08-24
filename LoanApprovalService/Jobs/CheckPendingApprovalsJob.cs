using Bank.Application.Interfaces;
using Bank.Shared.Events;
using LoanApprovalService.Entities;
using LoanApprovalService.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace LoanApprovalService.Jobs
{
	public class CheckPendingApprovalsJob
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IRabbitMQPublisher _publisher;

		public CheckPendingApprovalsJob(IServiceProvider serviceProvider, IRabbitMQPublisher publisher)
		{
			_serviceProvider = serviceProvider;
			_publisher = publisher;
		}

		public async Task CheckAndDeclineStaleRequests()
		{
			using var scope = _serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<LoanApprovalDbContext>();

			var cutoff = DateTime.UtcNow.AddHours(-24);
			var pending = await context.LoanApprovals
				.Where(x => x.Status == LoanStatus.PendingApproval && x.RequestedAt < cutoff)
				.ToListAsync();

			foreach (var approval in pending)
			{
				approval.Status = LoanStatus.Rejected;
				approval.RejectedAt = DateTime.UtcNow;
				approval.RejectionReason = "Auto-declined: No response within 24 hours";

				_publisher.Publish(new LoanRejectedEvent(approval.LoanId, approval.AccountId, approval.RejectionReason));
			}

			await context.SaveChangesAsync();
		}
	}
}
