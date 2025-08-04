using Bank.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Persistence
{
	public class ApplicationDbContext : IdentityDbContext<Account>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<Account> Accounts { get; set; }
		public DbSet<RefreshToken> RefreshTokens { get; set; }
		public DbSet<BankAccount> BankAccounts { get; set; }
		public DbSet<Transaction> Transactions { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Account>().ToTable("AspNetUsers");
			builder.Entity<Account>().HasNoDiscriminator();


			builder.Entity<BankAccount>(entity =>
			{
				entity.HasKey(ba => ba.Id);
				entity.Property(ba => ba.AccountNumber).IsRequired().HasMaxLength(20);
				entity.Property(ba => ba.Balance).HasColumnType("decimal(18,2)");
				entity.Property(ba => ba.Type).HasConversion<string>();
				entity.HasOne(ba => ba.Owner)
					  .WithMany(u => u.BankAccounts)
					  .HasForeignKey(ba => ba.OwnerId);
				entity.ToTable("BankAccounts");
			});

			builder.Entity<CheckingAccount>()
				.HasBaseType<BankAccount>();

			builder.Entity<SavingsAccount>()
				.HasBaseType<BankAccount>();

			builder.Entity<Transaction>(entity =>
			{
				entity.HasKey(t => t.Id);
				entity.Property(t => t.Amount).HasColumnType("decimal(18,2)");
				entity.Property(t => t.Type).HasConversion<string>();
				entity.HasOne(t => t.BankAccount)
					  .WithMany(ba => ba.Transactions)
					  .HasForeignKey(t => t.BankAccountId);
				entity.ToTable("Transactions");
			});

			builder.Entity<Account>()
			.HasMany(u => u.BankAccounts)
			.WithOne(ba => ba.Owner)
			.HasForeignKey(ba => ba.OwnerId);
		}



	}
}

