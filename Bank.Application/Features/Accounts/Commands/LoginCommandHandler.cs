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
	public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
	{
		private readonly IAuthService _authService;

		public LoginCommandHandler(IAuthService authService)
		{
			_authService = authService;
		}

		public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
		{
			return await _authService.LoginAsync(request.Username, request.Password);
		}
	}
}
