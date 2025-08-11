using Microsoft.EntityFrameworkCore;

namespace Bank.TransactionsLogService.Entities
{
	public class TransactionLogDbContext : DbContext
	{
		public TransactionLogDbContext(DbContextOptions<TransactionLogDbContext> options) : base(options) { }
		public DbSet<TransactionLog> TransactionLogs { get; set; }
	}
}
