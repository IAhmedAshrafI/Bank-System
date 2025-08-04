using Bank.Application.Features.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Accounts.Commands
{
	public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;
}
