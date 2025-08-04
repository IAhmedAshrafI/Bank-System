
using Bank.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public class Transaction
	{
		public Guid Id { get; set; }
		public Guid BankAccountId { get; set; }
		public decimal Amount { get; set; }
		public TransactionType Type { get; set; }
		public DateTime Timestamp { get; set; }

		// Navigation
		public BankAccount BankAccount { get; set; } = null!;
	}
}
