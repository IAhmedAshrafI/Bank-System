using Bank.TransactionsLogService.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bank.TransactionsLogService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TransactionLogsController : ControllerBase
	{
		private readonly TransactionLogDbContext _context;

		public TransactionLogsController(TransactionLogDbContext context)
		{
			_context = context;
		}

		[HttpGet("GetAllTransactionLogs")]
		public async Task<IActionResult> GetAllTransactionLogs()
		{
			var logs = await _context.TransactionLogs.ToListAsync();
			return Ok(logs);
		}
	}
}
