using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Jobs
{
	public class ApplyInterestJob
	{
		private readonly IBankAccountRepository _bankAccount;
		public ApplyInterestJob(IBankAccountRepository bankAccount)
		{
			_bankAccount = bankAccount;
		}

		public async Task ApplyInterestToSavingsAccounts()
		{

			var savingsAccounts = await _bankAccount.GetAllSavingsBankAccounts();

			foreach (var account in savingsAccounts)
			{
				account.ApplyInterest();
			}

			await _bankAccount.SaveChangesAsync();
		}
	}
}
