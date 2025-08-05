using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bank.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAccountTypeConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountTypeConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverdraftLimit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypeConfigs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AccountTypeConfigs",
                columns: new[] { "Id", "Description", "InterestRate", "IsActive", "Name", "OverdraftLimit", "Type" },
                values: new object[,]
                {
                    { 1, "Allows overdrafts up to $500", 0m, true, "Checking Account", 500m, 0 },
                    { 2, "Earns 2% interest, no overdrafts", 0.02m, true, "Savings Account", 0m, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountTypeConfigs");
        }
    }
}
