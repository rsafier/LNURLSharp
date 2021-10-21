using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LNURLSharp.Migrations
{
    public partial class Migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Invoices",
                newName: "ExpiryDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Invoices",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DescriptionHash",
                table: "Invoices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Expired",
                table: "Invoices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FallbackAddr",
                table: "Invoices",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "Invoices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RHashBase64",
                table: "Invoices",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "DescriptionHash",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Expired",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FallbackAddr",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "RHashBase64",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "Invoices",
                newName: "CreateDate");
        }
    }
}
