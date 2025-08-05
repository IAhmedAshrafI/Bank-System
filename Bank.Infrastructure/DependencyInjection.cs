using Bank.Application.Interfaces;
using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using Bank.Infrastructure.Persistence;
using Bank.Infrastructure.Repositories;
using Bank.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure
{
	public static class DependencyInjection
	{

		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			var connectionString = configuration.GetConnectionString("DefaultConnection");

			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			services.AddIdentity<Account, IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

			// 🔁 Repositories
			services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
			services.AddScoped<IBankAccountRepository, BankAccountRepository>();
			services.AddScoped<IAccountTypeService, AccountTypeService>();

			// 🔐 Auth Service
			services.AddScoped<IAuthService, AuthService>();



			return services;
		}
	}
}
