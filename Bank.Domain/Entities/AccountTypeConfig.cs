using Bank.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public class AccountTypeConfig
	{
		public int Id { get; set; }
		public AccountType Type { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal OverdraftLimit { get; set; }
		public decimal InterestRate { get; set; }
		public bool IsActive { get; set; } = true;
	}
}
