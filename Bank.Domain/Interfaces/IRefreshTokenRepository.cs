using Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Interfaces
{
	public interface IRefreshTokenRepository
	{
		Task<RefreshToken> GetByTokenAsync(string token, CancellationToken ct);
		Task AddAsync(RefreshToken token, CancellationToken ct);
		Task RevokeAsync(string token, CancellationToken ct);
	}
}
