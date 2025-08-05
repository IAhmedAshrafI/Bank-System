using Bank.Domain.Entities.Enums;
using Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Interfaces
{
	public interface IAccountTypeService
	{
		Task<AccountTypeConfig> GetConfigAsync(AccountType type, CancellationToken ct);
	}
}
