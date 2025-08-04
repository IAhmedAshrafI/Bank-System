using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
	public interface IBaseRepository<T> where T : class
	{
		Task<T> GetByIdAsync(object id, CancellationToken ct);
		Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
		Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
		Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
		Task AddAsync(T entity, CancellationToken ct);
		Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct);
		Task Update(T entity);
		Task Delete(T entity);
		Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
	}
}
