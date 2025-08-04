using Bank.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
	public interface IUserRepository
	{
		Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
		Task<Account> GetByUsernameAsync(string username, CancellationToken ct);
	}
}
