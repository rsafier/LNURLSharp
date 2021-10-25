using Microsoft.EntityFrameworkCore.Migrations;

namespace LNURLSharp.Migrations
{
    public partial class WithdrawSetup2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaySetups");

            migrationBuilder.CreateTable(
                name: "WithdrawSetups",
                columns: table => new
                {
                    WithdrawSetupId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MaxWithdrawable = table.Column<long>(type: "INTEGER", nullable: false),
                    MinWithdrawable = table.Column<long>(type: "INTEGER", nullable: false),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false),
                    K1 = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawSetups", x => x.WithdrawSetupId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WithdrawSetups");

            migrationBuilder.CreateTable(
                name: "PaySetups",
                columns: table => new
                {
                    PaySetupId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaySetups", x => x.PaySetupId);
                });
        }
    }
}
