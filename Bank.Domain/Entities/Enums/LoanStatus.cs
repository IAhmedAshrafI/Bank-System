using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities.Enums
{
	public enum LoanStatus
	{
		PendingApproval,
		Approved,
		Rejected,
		Active,
		Closed
	}
}
