using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Commands
{
	public record DepositCommand(Guid AccountId, decimal Amount) : IRequest;
}
