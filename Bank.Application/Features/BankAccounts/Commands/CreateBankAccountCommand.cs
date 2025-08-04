using Bank.Domain.Entities.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Commands
{
	public record CreateBankAccountCommand(
	string AccountNumber,
	AccountType Type,
	Guid OwnerId) : IRequest<Guid>;
}
