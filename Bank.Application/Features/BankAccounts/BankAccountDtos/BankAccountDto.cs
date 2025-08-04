using Bank.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.BankAccounts.BankAccountDtos
{
	public record BankAccountDto(
	Guid Id,
	string AccountNumber,
	decimal Balance,
	AccountType Type,
	DateTime CreatedAt);
}
