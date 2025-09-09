using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PennyPincher.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_CheckedAtUtc_column_to_statements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedAt",
                table: "Statements",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckedAt",
                table: "Statements");
        }
    }
}
