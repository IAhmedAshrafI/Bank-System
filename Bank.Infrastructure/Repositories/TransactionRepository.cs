using Bank.Domain.Entities;
using Bank.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Repositories
{
	public class TransactionRepository : BaseRepository<Transaction>
	{
		public TransactionRepository(ApplicationDbContext context) : base(context) { }
	}
}
