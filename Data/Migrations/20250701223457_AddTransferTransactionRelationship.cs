using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferTransactionRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransferId",
                table: "Transaction",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_TransferId",
                table: "Transaction",
                column: "TransferId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Transfer_TransferId",
                table: "Transaction",
                column: "TransferId",
                principalTable: "Transfer",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Transfer_TransferId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_TransferId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "TransferId",
                table: "Transaction");
        }
    }
}
