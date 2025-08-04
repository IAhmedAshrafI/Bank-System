using Bank.Domain.Entities;
using Bank.Domain.Entities.Enums;
using Bank.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Commands
{
	public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand>
	{
		private readonly IBankAccountRepository _bankAccount;

		public WithdrawCommandHandler(IBankAccountRepository bankAccount)
		{
			_bankAccount = bankAccount;
		}

		public async Task Handle(WithdrawCommand request, CancellationToken cancellationToken)
		{
        if (request.Amount <= 0)
            throw new ArgumentException("Withdrawal amount must be positive.");

        var account = await _bankAccount.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
            throw new KeyNotFoundException($"Account not found with ID: {request.AccountId}");

        account.Withdraw(request.Amount);

        await _bankAccount.SaveChangesAsync(cancellationToken);
		}
	}
} 