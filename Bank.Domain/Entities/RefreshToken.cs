using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Entities
{
	public class RefreshToken
	{
		public int Id { get; set; }
		public string Token { get; set; }
		public string AccountId { get; set; }
		public DateTime ExpiresAt { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public bool Revoked { get; set; }
	}
}
