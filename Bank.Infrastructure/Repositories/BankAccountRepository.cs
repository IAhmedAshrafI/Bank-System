using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Repositories
{
	public class BankAccountRepository : IBankAccountRepository
	{
		private readonly ApplicationDbContext _context;

		public BankAccountRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken ct)
		{
			var account = await _context.BankAccounts
					 .Include(a => a.Transactions)
					 .SingleOrDefaultAsync(a => a.Id == id, ct);
			
			return account;
		}

		public async Task<List<SavingsAccount>> GetAllSavingsBankAccounts()
		{
			return await _context.BankAccounts
			.OfType<SavingsAccount>()
			.ToListAsync();
		}

		public async Task AddAsync(BankAccount account, CancellationToken ct)
			=> await _context.BankAccounts.AddAsync(account, ct);

		public async Task UpdateAsync(BankAccount account, CancellationToken ct)
		{
			var entry = _context.Entry(account);
			if (entry.State == EntityState.Detached)
			{
				_context.BankAccounts.Attach(account);
			}
			entry.State = EntityState.Modified;
		}

		public async Task DeleteAsync(BankAccount account, CancellationToken ct)
			=> _context.BankAccounts.Remove(account);

		public async Task<int> SaveChangesAsync(CancellationToken ct = default)
		{
			try
			{
				return await _context.SaveChangesAsync(ct);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				throw new InvalidOperationException($"Concurrency error occurred. The entity may have been modified by another process. Details: {ex.Message}", ex);
			}
		}
	}
}
