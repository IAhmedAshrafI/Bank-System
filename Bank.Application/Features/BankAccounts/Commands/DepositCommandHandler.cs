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
	public class DepositCommandHandler : IRequestHandler<DepositCommand>
	{
		private readonly IBankAccountRepository _bankAccount;

		public DepositCommandHandler(IBankAccountRepository bankAccount)
		{
			_bankAccount = bankAccount;
		}

		public async Task Handle(DepositCommand request, CancellationToken cancellationToken)
		{
        if (request.Amount <= 0)
            throw new ArgumentException("Deposit amount must be positive.");

        var account = await _bankAccount.GetByIdAsync(request.AccountId, cancellationToken);
        if (account == null)
            throw new KeyNotFoundException($"Account not found with ID: {request.AccountId}");

        if (account.Id == Guid.Empty)
            throw new InvalidOperationException("Account ID is not valid.");

        Console.WriteLine($"Processing deposit for account {account.Id} with current balance: {account.Balance}");

        account.Deposit(request.Amount);

        Console.WriteLine($"New balance after deposit: {account.Balance}");

        try
        {
            await _bankAccount.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Successfully saved deposit transaction for account {account.Id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving deposit: {ex.Message}");
            throw new InvalidOperationException($"Failed to save deposit transaction. Account ID: {request.AccountId}, Amount: {request.Amount}. Error: {ex.Message}", ex);
        }
		}
	}
}
