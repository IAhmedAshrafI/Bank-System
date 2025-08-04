using Bank.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public abstract class BankAccount
	{
		public Guid Id { get; protected set; }
		public string AccountNumber { get;  set; } = string.Empty;
		public decimal Balance { get;  set; }
		public AccountType Type { get;  set; }
		public DateTime CreatedAt { get; set; }

		// FK
		public string OwnerId { get; set; } = string.Empty;

		// Navigation
		public Account Owner { get; protected set; } = null!;
		public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();

		public virtual void Deposit(decimal amount)
		{
			if (amount <= 0) throw new ArgumentException("Deposit must be positive");

			Balance += amount;
			AddTransaction(TransactionType.Deposit, amount);
		}

		public virtual void Withdraw(decimal amount)
		{
			if (amount <= 0) throw new ArgumentException("Withdrawal must be positive");
			if (!CanWithdraw(amount)) throw new InvalidOperationException("Insufficient funds");

			Balance -= amount;
			AddTransaction(TransactionType.Withdrawal, amount);
		}

		public abstract bool CanWithdraw(decimal amount);

		protected void AddTransaction(TransactionType type, decimal amount)
		{
			var transaction = new Transaction
			{
				BankAccountId = this.Id,
				Amount = amount,
				Type = type,
				Timestamp = DateTime.UtcNow
			};
			Transactions.Add(transaction);
		}

		protected BankAccount() 
		{
			Id = Guid.NewGuid();
			CreatedAt = DateTime.UtcNow;
		}
	}
}
