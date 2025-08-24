using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Shared.Events
{
	public record LoanRejectedEvent(Guid LoanId, Guid AccountId, string Reason);
}
