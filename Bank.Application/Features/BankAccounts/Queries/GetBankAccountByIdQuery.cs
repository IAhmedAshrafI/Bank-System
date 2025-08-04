using Bank.Application.Features.BankAccounts.BankAccountDtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Queries
{
	public record GetBankAccountByIdQuery(Guid Id) : IRequest<BankAccountDto>;
}
