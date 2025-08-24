using Microsoft.EntityFrameworkCore;

namespace LoanApprovalService.Entities
{
	public class LoanApprovalDbContext : DbContext
	{
		public LoanApprovalDbContext(DbContextOptions<LoanApprovalDbContext> options) : base(options) { }

		public DbSet<LoanApproval> LoanApprovals { get; set; }
	}
}
