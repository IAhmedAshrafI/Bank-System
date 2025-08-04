using Bank.Domain.Interfaces;
using Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Repositories
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		protected readonly ApplicationDbContext Context;
		protected readonly DbSet<T> DbSet;

		public BaseRepository(ApplicationDbContext context)
		{
			Context = context;
			DbSet = context.Set<T>();
		}

		public async Task<T> GetByIdAsync(object id, CancellationToken ct)
			=> await DbSet.FindAsync(new[] { id }, ct);

		public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
			=> await DbSet.SingleOrDefaultAsync(predicate, ct);

		public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
			=> await DbSet.ToListAsync(ct);

		public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
			=> await DbSet.Where(predicate).ToListAsync(ct);

		public async Task AddAsync(T entity, CancellationToken ct)
		{
			await DbSet.AddAsync(entity, ct);
			await Context.SaveChangesAsync(ct);
		}

		public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct)
		{
			await DbSet.AddRangeAsync(entities, ct);
			await Context.SaveChangesAsync(ct);
		}

		public async Task Update(T entity)
		{
			DbSet.Update(entity);
			await Context.SaveChangesAsync();
		}

		public async Task Delete(T entity)
		{
			DbSet.Remove(entity);
			await Context.SaveChangesAsync();
		}

		public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct)
			=> await DbSet.AnyAsync(predicate, ct);
	}
}
