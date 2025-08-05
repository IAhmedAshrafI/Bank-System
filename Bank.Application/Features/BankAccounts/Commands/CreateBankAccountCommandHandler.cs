using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Entities.Enums;
using Bank.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Commands
{
	public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, Guid>
	{
		private readonly IBankAccountRepository _bankAccountRepository;
		private readonly IAccountTypeService _accountTypeService;

		public CreateBankAccountCommandHandler(IBankAccountRepository bankAccountRepository, IAccountTypeService accountTypeService)
		{
			_bankAccountRepository = bankAccountRepository;
			_accountTypeService = accountTypeService;
		}

		public async Task<Guid> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
		{
			var config = await _accountTypeService.GetConfigAsync(request.Type, cancellationToken);

			BankAccount account = request.Type switch
			{
				AccountType.Checking => new CheckingAccount()
				{
					OverdraftLimit = config.OverdraftLimit
				},

				AccountType.Savings => new SavingsAccount()
				{
					InterestRate = config.InterestRate
				},
				_ => throw new ArgumentException("Invalid account type")
			};

			account.AccountNumber = request.AccountNumber;
			account.OwnerId = request.OwnerId.ToString();

			await _bankAccountRepository.AddAsync(account, cancellationToken);
			await _bankAccountRepository.SaveChangesAsync();
			return account.Id;
		}
	}
}
