using Bank.Application.Features.Loans.Dtos;
using Bank.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Loans.Queries
{
	public record GetLoanById(Guid Id) : IRequest<LoanDto>
	{
	}
}
