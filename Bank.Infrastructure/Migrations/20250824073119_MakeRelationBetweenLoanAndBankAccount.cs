using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeRelationBetweenLoanAndBankAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_AccountId1",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_AccountId1",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "AccountId1",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Loans",
                newName: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BankAccountId",
                table: "Loans",
                column: "BankAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_BankAccounts_BankAccountId",
                table: "Loans",
                column: "BankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_BankAccounts_BankAccountId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BankAccountId",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "BankAccountId",
                table: "Loans",
                newName: "AccountId");

            migrationBuilder.AddColumn<string>(
                name: "AccountId1",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_AccountId1",
                table: "Loans",
                column: "AccountId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_AccountId1",
                table: "Loans",
                column: "AccountId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
