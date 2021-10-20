using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LNURLSharp.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LNDServers",
                columns: table => new
                {
                    Pubkey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LNDServers", x => x.Pubkey);
                });

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

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Comment = table.Column<string>(type: "TEXT", nullable: true),
                    Metadata = table.Column<string>(type: "TEXT", nullable: true),
                    Payreq = table.Column<string>(type: "TEXT", nullable: true),
                    Username = table.Column<string>(type: "TEXT", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LNDServerPubkey = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_LNDServers_LNDServerPubkey",
                        column: x => x.LNDServerPubkey,
                        principalTable: "LNDServers",
                        principalColumn: "Pubkey",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_LNDServerPubkey",
                table: "Invoices",
                column: "LNDServerPubkey");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Payreq",
                table: "Invoices",
                column: "Payreq",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Username",
                table: "Invoices",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_LNDServers_Pubkey",
                table: "LNDServers",
                column: "Pubkey",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PaySetups");

            migrationBuilder.DropTable(
                name: "LNDServers");
        }
    }
}
