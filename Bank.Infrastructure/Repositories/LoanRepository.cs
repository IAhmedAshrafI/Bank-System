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
	public class LoanRepository : ILoanRepository
	{
		public readonly ApplicationDbContext _context;

		public LoanRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Loan loan, CancellationToken ct)
		{
			await _context.Loans.AddAsync(loan);
			await _context.SaveChangesAsync(ct);
		}

		public async Task<Loan?> GetLoanById(Guid id, CancellationToken ct)
		{
			var loan = await _context.Loans
					 .SingleOrDefaultAsync(a => a.Id == id, ct);

			return loan;
		}
	}
}
