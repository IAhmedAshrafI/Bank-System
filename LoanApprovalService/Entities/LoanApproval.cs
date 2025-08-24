using LoanApprovalService.Entities.Enums;

namespace LoanApprovalService.Entities
{
	public class LoanApproval
	{
		public Guid Id { get; set; }
		public Guid LoanId { get; set; }
		public Guid AccountId { get; set; }
		public decimal Amount { get; set; }
		public int TermMonths { get; set; }
		public decimal AnnualInterestRate { get; set; }
		public DateTime RequestedAt { get; set; }
		public LoanStatus Status { get; set; } = LoanStatus.PendingApproval;
		public DateTime? ApprovedAt { get; set; }
		public DateTime? RejectedAt { get; set; }
		public string? RejectionReason { get; set; }
		public Guid? ApprovedById { get; set; }
		public Guid? RejectedById { get; set; }
	}
}
