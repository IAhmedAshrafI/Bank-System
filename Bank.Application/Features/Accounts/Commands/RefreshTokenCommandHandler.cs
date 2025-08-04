using Bank.Application.Features.Accounts.Dtos;
using Bank.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Accounts.Commands
{
	public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
	{
		private readonly IAuthService _authService;

		public RefreshTokenCommandHandler(IAuthService authService)
		{
			_authService = authService;
		}

		public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken ct)
		{
			return await _authService.RefreshTokenAsync(request.RefreshToken);
		}
	}
}
