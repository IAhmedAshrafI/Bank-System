using Bank.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public class Loan
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
		public LoanStatus Status { get; set; } = LoanStatus.PendingApproval;

		public Guid BankAccountId { get; set; }
		public BankAccount BankAccount { get; set; } = null!;
		public ICollection<LoanPayment> Payments { get; private set; } = new List<LoanPayment>();
	}
}
