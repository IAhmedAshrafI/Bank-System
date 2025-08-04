using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Repositories
{
	public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
	{
		public RefreshTokenRepository(ApplicationDbContext context) : base(context) { }

		public async Task<RefreshToken> GetByTokenAsync(string token, CancellationToken ct)
			=> await GetSingleAsync(rt => rt.Token == token, ct);

		public async Task AddAsync(RefreshToken token, CancellationToken ct)
			=> await base.AddAsync(token, ct);

		public async Task RevokeAsync(string token, CancellationToken ct)
		{
			var entity = await GetByTokenAsync(token, ct);
			if (entity != null)
			{
				entity.Revoked = true;
				await Update(entity);
			}
		}
	}
}
