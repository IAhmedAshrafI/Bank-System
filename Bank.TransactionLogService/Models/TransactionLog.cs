using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.TransactionLogService.Models
{
	public class TransactionLog
	{
		public Guid Id { get; set; }
		public Guid AccountId { get; set; }
		public decimal Amount { get; set; }
		public string Type { get; set; } = string.Empty;
		public DateTime Timestamp { get; set; }
		public string SourceService { get; set; } = string.Empty;
	}
}
