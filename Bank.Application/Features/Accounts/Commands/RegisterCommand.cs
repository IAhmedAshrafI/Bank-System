using Bank.Application.Features.Accounts.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Accounts.Commands
{
	public record RegisterCommand(
	string Username,
	string Email,
	string Password,
	string Role = "User")
	: IRequest<AuthResponseDto>;
}
