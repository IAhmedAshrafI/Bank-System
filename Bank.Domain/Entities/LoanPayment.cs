using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public class LoanPayment
	{
		public Guid Id { get; set; }
		public Guid LoanId { get; set; }
		public decimal Amount { get; set; }
		public DateTime DueDate { get; set; }
		public DateTime? PaidDate { get; set; }
		public bool IsPaid { get; set; }
		public bool IsLateFeeApplied { get; set; }
		public decimal LateFee { get; set; } = 25.00m;

		public Loan Loan { get; set; } = null!;
	}
}
