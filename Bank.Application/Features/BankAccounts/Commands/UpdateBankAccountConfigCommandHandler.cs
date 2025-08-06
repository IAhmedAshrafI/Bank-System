using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Commands
{
	public class UpdateBankAccountConfigCommandHandler : IRequestHandler<UpdateBankAccountConfigCommand, AccountTypeConfig>
	{
		private readonly IAccountTypeService _accountTypeService;

		public UpdateBankAccountConfigCommandHandler(IAccountTypeService accountTypeService)
		{
			_accountTypeService = accountTypeService;
		}

		public async Task<AccountTypeConfig> Handle(UpdateBankAccountConfigCommand request, CancellationToken cancellationToken)
		{
			var AccountType = new AccountTypeConfig
			{
				InterestRate = request.InterestRate,
				OverdraftLimit = request.OverdraftLimit
			};
			var AccountTypeConfig = await _accountTypeService.UpdateConfigAsync(request.Id, AccountType);
			return AccountTypeConfig;
		}
	}
}
