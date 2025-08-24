using Bank.Application.Features.Loans.Dtos;
using Bank.Application.Features.Loans.Queries;
using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Loans.Commands
{
	public class GetLoanByIdCommandHandler : IRequestHandler<GetLoanById, LoanDto>
	{
		private readonly ILoanRepository _loan;

		public GetLoanByIdCommandHandler(ILoanRepository loan)
		{
			_loan = loan;
		}

		public async Task<LoanDto> Handle(GetLoanById request, CancellationToken cancellationToken)
		{
			var loan = await _loan.GetLoanById(request.Id, cancellationToken);

			if (loan == null)
			{
				throw new KeyNotFoundException($"Loan not found with ID: {request.Id}");
			}

			return new LoanDto
			{
				Id = loan.Id,
				Principal = loan.Principal,
				InterestRate = loan.InterestRate,
				RepaymentPeriodMonths = loan.RepaymentPeriodMonths,
				StartDate = loan.StartDate,
				DueDate = loan.DueDate,
				MonthlyPayment = loan.MonthlyPayment,
				RemainingBalance = loan.RemainingBalance,
				PaymentsMade = loan.PaymentsMade,
				Status = loan.Status.ToString(),
				BankAccountId = loan.BankAccountId
			};


		}
	}
	
}
