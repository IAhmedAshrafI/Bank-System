using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Features.Loans.Dtos
{
	public class LoanDto
	{
		public Guid Id { get; set; }
		public decimal Principal { get; set; }
		public decimal InterestRate { get; set; }
		public int RepaymentPeriodMonths { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime DueDate { get; set; }
		public decimal MonthlyPayment { get; set; }
		public decimal RemainingBalance { get; set; }
		public int PaymentsMade { get; set; }

		// String keeps your API stable if enum values change order later
		public string Status { get; set; } = default!;

		public Guid BankAccountId { get; set; }
	}
}
