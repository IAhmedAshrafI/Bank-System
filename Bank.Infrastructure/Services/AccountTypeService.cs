using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Entities.Enums;
using Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Services
{
	public class AccountTypeService : IAccountTypeService
	{
		private readonly ApplicationDbContext _context;
		private readonly IDistributedCache _cache;

		public AccountTypeService(ApplicationDbContext context, IDistributedCache cache)
		{
			_context = context;
			_cache = cache;
		}

		public async Task<AccountTypeConfig> GetConfigAsync(AccountType type, CancellationToken ct)
		{
			var cacheKey = $"accounttype:{type}";

			var cached = await _cache.GetStringAsync(cacheKey, ct);
			if (!string.IsNullOrEmpty(cached))
			{
				return JsonSerializer.Deserialize<AccountTypeConfig>(cached)!;
			}

			var config = await _context.AccountTypeConfigs
				.FirstOrDefaultAsync(x => x.Type == type, ct);

			if (config == null)
				throw new KeyNotFoundException($"Account type config for {type} not found");

			var options = new DistributedCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromHours(24));

			await _cache.SetStringAsync(
				cacheKey,
				JsonSerializer.Serialize(config),
				options,
				ct);

			return config;
		}

		public async Task<AccountTypeConfig> UpdateConfigAsync(int Id, AccountTypeConfig accountType)
		{
			var checkAccount = await _context.AccountTypeConfigs
				.FirstOrDefaultAsync(x => x.Id == Id);

			if (checkAccount == null)
			{
				throw new KeyNotFoundException($"Account type config not found");
			}

			checkAccount.OverdraftLimit = accountType.OverdraftLimit;
			checkAccount.InterestRate = accountType.InterestRate;
			 _context.AccountTypeConfigs.Update(checkAccount);
			 await _context.SaveChangesAsync();

			if (await _cache.GetStringAsync($"accounttype:{checkAccount.Type}") != null)
			{
				await _cache.RemoveAsync($"accounttype:{checkAccount.Type}");
			}

			var cacheKey = $"accounttype:{checkAccount.Type}";
			var options = new DistributedCacheEntryOptions()
				.SetAbsoluteExpiration(TimeSpan.FromHours(24));
			await _cache.SetStringAsync(
				cacheKey,
				JsonSerializer.Serialize(checkAccount),
				options
			);
			return checkAccount;
		}
	}
}

