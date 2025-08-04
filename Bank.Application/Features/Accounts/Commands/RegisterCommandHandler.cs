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
	public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
	{
		private readonly IAuthService _authService;

		public RegisterCommandHandler(IAuthService authService)
		{
			_authService = authService;
		}

		public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken ct)
		{
			return await _authService.RegisterAsync(request.Username, request.Email, request.Password, request.Role);
		}
	}
}
