using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Loans.Commands
{
	public record CreateLoanCommand(
	Guid AccountId,
	decimal Amount,
	int TermMonths,
	decimal AnnualInterestRate) : IRequest<Guid>;
}
