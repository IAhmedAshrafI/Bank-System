using Bank.Application.Features.BankAccounts.BankAccountDtos;
using Bank.Application.Features.BankAccounts.Queries;
using Bank.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Commands
{
	public class GetBankAccountByIdQueryHandler : IRequestHandler<GetBankAccountByIdQuery, BankAccountDto>
	{
		private readonly IBankAccountRepository _bankAccountRepository;
		public GetBankAccountByIdQueryHandler(IBankAccountRepository bankAccountRepository)
		{
			_bankAccountRepository = bankAccountRepository;
		}

		public async Task<BankAccountDto> Handle(GetBankAccountByIdQuery request, CancellationToken cancellationToken)
		{
			var account = await _bankAccountRepository.GetByIdAsync(request.Id, cancellationToken);
			if (account == null)
				throw new KeyNotFoundException($"Bank account with ID {request.Id} not found.");

			return new BankAccountDto(
				account.Id,
				account.AccountNumber,
				account.Balance,
				account.Type,
				account.CreatedAt
			);
		}
	}
}
