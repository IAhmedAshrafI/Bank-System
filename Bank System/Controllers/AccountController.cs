using Bank.Application.Features.Accounts.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bank_System.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly ISender _mediator;

		public AccountController(ISender mediator) => _mediator = mediator;

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterCommand command) =>
			Ok(await _mediator.Send(command));

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginCommand command) =>
			Ok(await _mediator.Send(command));

		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh(RefreshTokenCommand command) =>
			Ok(await _mediator.Send(command));
	}
}
