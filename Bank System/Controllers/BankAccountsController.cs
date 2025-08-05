using Bank.Application.Features.BankAccounts.BankAccountDtos;
using Bank.Application.Features.BankAccounts.Commands;
using Bank.Application.Features.BankAccounts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bank_System.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BankAccountsController : ControllerBase
	{
		private readonly ISender _mediator;

		public BankAccountsController(ISender mediator) => _mediator = mediator;

		[HttpPost]
		public async Task<IActionResult> Create(CreateBankAccountCommand command)
		{
			
			var id = await _mediator.Send(command);
			return CreatedAtAction(nameof(GetById), new { id }, id);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<BankAccountDto>> GetById(Guid id)
		{
			var query = new GetBankAccountByIdQuery(id);
			var result = await _mediator.Send(query);
			return Ok(result);
		}

		[HttpPost("deposit")]
		public async Task<IActionResult> Deposit(DepositCommand command)
		{
			await _mediator.Send(command);
			return Ok("Deposit Done Succsessfully");
		}

		[HttpPost("withdraw")]
		public async Task<IActionResult> Withdraw(WithdrawCommand command)
		{
			await _mediator.Send(command);
			return Ok("Withdraw Done Succsessfully");
		}

		[HttpPost("transfer")]
		public async Task<IActionResult> Transfer(TransferCommand command)
		{
			await _mediator.Send(command);
			return Ok("Transfer Done Succsessfully");
		}
	}
}
