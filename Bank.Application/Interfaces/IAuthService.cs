using Bank.Application.Features.Accounts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Interfaces
{
	public interface IAuthService
	{
		Task<AuthResponseDto> RegisterAsync(string username, string email, string password, string role);
		Task<AuthResponseDto> LoginAsync(string username, string password);
		Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
	}
}
