using Bank.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public class SavingsAccount : BankAccount
	{
		public decimal InterestRate { get; set; } = 0.02m;

		public SavingsAccount()
		{
			Type = AccountType.Savings;
		}

		public override bool CanWithdraw(decimal amount)
			=> Balance >= amount;

		public void ApplyInterest()
		{
			var interest = Balance * InterestRate;
			Balance += interest;
			AddTransaction(TransactionType.Interest, interest);
		}
	}
}
