using Bank.Domain.Entities;
using Bank.Domain.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
		public DbSet<AccountTypeConfig> AccountTypeConfigs { get; set; }
		public DbSet<Loan> Loans { get; set; }
		public DbSet<LoanPayment> LoanPayments { get; set; }

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

			builder.Entity<Loan>(b =>
			{
				b.HasOne(l => l.BankAccount)
				 .WithMany(a => a.Loans)
				 .HasForeignKey(l => l.BankAccountId)
				 .IsRequired()
				 .OnDelete(DeleteBehavior.Restrict);
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

			

			builder.Entity<LoanPayment>(entity =>
			{
				entity.HasKey(p => p.Id);
				entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
				entity.Property(p => p.LateFee).HasColumnType("decimal(18,2)");
			});

			



			builder.Entity<AccountTypeConfig>().HasData(
		new AccountTypeConfig
		{
			Id = 1,
			Type = AccountType.Checking,
			Name = "Checking Account",
			Description = "Allows overdrafts up to $500",
			OverdraftLimit = 500,
			InterestRate = 0
		},
		new AccountTypeConfig
		{
			Id = 2,
			Type = AccountType.Savings,
			Name = "Savings Account",
			Description = "Earns 2% interest, no overdrafts",
			OverdraftLimit = 0,
			InterestRate = 0.02m
		}
	);
		}



	}
}

