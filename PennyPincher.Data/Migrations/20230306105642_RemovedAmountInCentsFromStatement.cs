using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PennyPincher.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedAmountInCentsFromStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountInCents",
                table: "Statements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmountInCents",
                table: "Statements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
