using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Shared.Events
{
	public record LoanApprovedEvent(Guid LoanId, Guid AccountId, decimal Amount);
}
