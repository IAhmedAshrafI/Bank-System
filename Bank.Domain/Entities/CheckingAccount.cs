using Bank.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public class CheckingAccount : BankAccount
	{
		public decimal OverdraftLimit { get; private set; } = 500;

		public CheckingAccount()
		{
			Type = AccountType.Checking;
		}

		public override bool CanWithdraw(decimal amount)
			=> Balance - amount >= -OverdraftLimit;
	}
}
