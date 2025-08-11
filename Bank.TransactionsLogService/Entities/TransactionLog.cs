namespace Bank.TransactionsLogService.Entities
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
