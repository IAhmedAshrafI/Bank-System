using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Domain.Constants
{
	public static class Roles
	{
		public const string Admin = "Admin";
		public const string User = "User";

		public static readonly string[] All =
		{
		Admin,
		User,
	};
	}
}
