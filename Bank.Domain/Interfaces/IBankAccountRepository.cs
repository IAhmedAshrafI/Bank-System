using Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
	public interface IBankAccountRepository
	{
		Task<BankAccount?> GetByIdAsync(Guid id, CancellationToken ct);
		Task AddAsync(BankAccount account, CancellationToken ct);
		Task UpdateAsync(BankAccount account, CancellationToken ct);
		Task DeleteAsync(BankAccount account, CancellationToken ct);
		Task<int> SaveChangesAsync(CancellationToken ct = default);
		Task<List<SavingsAccount>> GetAllSavingsBankAccounts();
	}
}
