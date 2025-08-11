using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Shared.Events
{
	public record TransactionCreatedEvent
	(
		Guid Id,
		Guid AccountId,
		decimal Amount,
		string Type,
		DateTime Timestamp,
		string SourceService = "BankingService"
	);
}
