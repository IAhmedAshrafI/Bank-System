using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Repositories
{
	public class UserRepository : BaseRepository<Account>, IUserRepository
	{
		public UserRepository(ApplicationDbContext context) : base(context) { }

		public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
			=> await ExistsAsync(u => u.Email == email, ct);

		public async Task<Account> GetByUsernameAsync(string username, CancellationToken ct)
			=> await GetSingleAsync(u => u.UserName == username, ct);
	}
}
