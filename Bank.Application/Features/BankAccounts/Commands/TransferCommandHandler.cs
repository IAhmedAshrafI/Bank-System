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
	public class TransferCommandHandler : IRequestHandler<TransferCommand>
	{
		private readonly IBankAccountRepository _bankAccount;

		public TransferCommandHandler(IBankAccountRepository bankAccount)
		{
			_bankAccount = bankAccount;
		}

		public async Task Handle(TransferCommand request, CancellationToken cancellationToken)
		{
        if (request.Amount <= 0)
            throw new ArgumentException("Transfer amount must be positive.");

        if (request.FromId == request.ToId)
            throw new ArgumentException("Cannot transfer to the same account.");

        var fromAccount = await _bankAccount.GetByIdAsync(request.FromId, cancellationToken);
        if (fromAccount == null)
            throw new KeyNotFoundException($"Source account not found with ID: {request.FromId}");

        var toAccount = await _bankAccount.GetByIdAsync(request.ToId, cancellationToken);
        if (toAccount == null)
            throw new KeyNotFoundException($"Destination account not found with ID: {request.ToId}");

        fromAccount.Withdraw(request.Amount);
        toAccount.Deposit(request.Amount);

        await _bankAccount.SaveChangesAsync(cancellationToken);
		}
	}
} 