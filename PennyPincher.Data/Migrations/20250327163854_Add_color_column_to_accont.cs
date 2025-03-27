using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PennyPincher.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_color_column_to_accont : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "Accounts",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "Accounts");
        }
    }
}
