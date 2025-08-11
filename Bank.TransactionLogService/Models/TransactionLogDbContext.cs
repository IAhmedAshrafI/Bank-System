using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.TransactionLogService.Models
{
	public class TransactionLogDbContext : DbContext
	{
		public TransactionLogDbContext(DbContextOptions<TransactionLogDbContext> options) : base(options) { }
		public DbSet<TransactionLog> TransactionLogs { get; set; }
	}
}
