using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace transactionApi.Migrations
{
    /// <inheritdoc />
    public partial class migracija4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "receiverAccount",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "senderAccount",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "receiverAccount",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "senderAccount",
                table: "Transaction");
        }
    }
}
