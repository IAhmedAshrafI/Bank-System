using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Shared.Events
{
	public static class QueueNames
	{
		public const string TransactionLog = "transaction-log-queue";
		public const string LoanApprovals = "loan-approvals";
		public const string LoanStatusUpdates = "loan-status-updates";
	}
}
