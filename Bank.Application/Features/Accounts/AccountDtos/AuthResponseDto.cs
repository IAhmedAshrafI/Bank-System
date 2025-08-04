using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Accounts.Dtos
{
	public record AuthResponseDto(
	string AccessToken,
	string RefreshToken,
	DateTime ExpiresAt);
}
