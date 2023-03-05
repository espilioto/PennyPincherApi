using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PennyPincher.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndAccountToStatement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Statements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Statements",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_AccountId",
                table: "Statements",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Statements_UserId",
                table: "Statements",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Statements_Accounts_AccountId",
                table: "Statements",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Statements_AspNetUsers_UserId",
                table: "Statements",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Statements_Accounts_AccountId",
                table: "Statements");

            migrationBuilder.DropForeignKey(
                name: "FK_Statements_AspNetUsers_UserId",
                table: "Statements");

            migrationBuilder.DropIndex(
                name: "IX_Statements_AccountId",
                table: "Statements");

            migrationBuilder.DropIndex(
                name: "IX_Statements_UserId",
                table: "Statements");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Statements");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Statements");
        }
    }
}
