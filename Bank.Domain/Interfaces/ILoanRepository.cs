using Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
	public interface ILoanRepository
	{
		Task<Loan?> GetLoanById(Guid id, CancellationToken ct);
		Task AddAsync(Loan loan, CancellationToken ct);
	}
}
