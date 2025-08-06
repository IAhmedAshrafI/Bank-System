using Bank.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.Commands
{
	public record UpdateBankAccountConfigCommand
	(
		int Id,
		decimal OverdraftLimit,
		decimal InterestRate
	) : IRequest<AccountTypeConfig>;

}
