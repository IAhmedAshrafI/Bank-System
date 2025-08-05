using Bank.Domain.Entities.Enums;
using Bank.Domain.Entities;
using Bank.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrastructure.Services
{
	public class SeedService
	{
		private readonly ApplicationDbContext _context;

		public SeedService(ApplicationDbContext context) => _context = context;

		public async Task SeedAccountTypesAsync()
		{
			if (await _context.AccountTypeConfigs.AnyAsync()) return;

			var types = new List<AccountTypeConfig>
		{
			new()
			{
				Type = AccountType.Checking,
				Name = "Checking Account",
				Description = "Allows overdrafts up to $500",
				OverdraftLimit = 500,
				InterestRate = 0
			},
			new()
			{
				Type = AccountType.Savings,
				Name = "Savings Account",
				Description = "Earns 2% interest, no overdrafts",
				OverdraftLimit = 0,
				InterestRate = 0.02m
			}
		};

			await _context.AccountTypeConfigs.AddRangeAsync(types);
			await _context.SaveChangesAsync();
		}
	}
}
