using Bank.Application.Interfaces;
using Bank.Domain.Entities.Enums;
using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using Bank.Shared.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Bank.Application.Features.Loans.Commands
{
	public class CreateLoanCommandHandler : IRequestHandler<CreateLoanCommand, Guid>
	{
		private readonly ILoanRepository _loan;
		private readonly IBankAccountRepository _bankAccount;
		private readonly IRabbitMQPublisher _rabbitMQPublisher;

		public CreateLoanCommandHandler(ILoanRepository loan, IRabbitMQPublisher rabbitMQPublisher, IBankAccountRepository bankAccount)
		{
			_loan = loan;
			_rabbitMQPublisher = rabbitMQPublisher;
			_bankAccount = bankAccount;
		}

		public async Task<Guid> Handle(CreateLoanCommand request, CancellationToken ct)
		{
			if (request.Amount <= 0) throw new ArgumentException("Loan amount must be positive.");
			if (request.TermMonths <= 0 || request.TermMonths > 120)
				throw new ArgumentException("Term must be between 1 and 120 months.");
			if (request.AnnualInterestRate < 0.01m || request.AnnualInterestRate > 0.30m)
				throw new ArgumentException("Interest rate must be between 1% and 30%.");

			var account = await _bankAccount.GetByIdAsync(request.AccountId, ct); 
			if (account is null) throw new KeyNotFoundException($"Account not found with ID: {request.AccountId}");

			var nowUtc = DateTime.UtcNow;
			var monthlyPmt = CalculateMonthlyPayment(request.Amount, request.AnnualInterestRate, request.TermMonths);

			var loan = new Loan
			{
				Principal = request.Amount,
				InterestRate = request.AnnualInterestRate,
				RepaymentPeriodMonths = request.TermMonths,
				StartDate = nowUtc,
				DueDate = nowUtc.AddMonths(request.TermMonths),
				MonthlyPayment = monthlyPmt,
				RemainingBalance = request.Amount,
				PaymentsMade = 0,
				Status = LoanStatus.PendingApproval,
				BankAccountId = account.Id,
				BankAccount = account
			};

			await _loan.AddAsync(loan, ct);

			_rabbitMQPublisher.Publish(new LoanRequestedEvent(
				LoanId: loan.Id,
				AccountId: request.AccountId,
				Amount: request.Amount,
				TermMonths: request.TermMonths,
				AnnualInterestRate: request.AnnualInterestRate,
				RequestedAt: nowUtc
			));

			return loan.Id;
		}

		private static decimal CalculateMonthlyPayment(decimal principal, decimal annualRate, int termMonths)
		{
			if (termMonths <= 0) throw new ArgumentOutOfRangeException(nameof(termMonths));
			var r = (double)annualRate / 12.0;
			if (Math.Abs(r) < 1e-12)
				return Math.Round(principal / termMonths, 2, MidpointRounding.AwayFromZero);

			var pmt = (double)principal * r / (1 - Math.Pow(1 + r, -termMonths));
			return Math.Round((decimal)pmt, 2, MidpointRounding.AwayFromZero);
		}
	}
}
