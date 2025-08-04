using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bank.Domain.Entities
{
	public class Account : IdentityUser
	{
		public ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();
	}
}
