using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegulatoryComplianceApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBillNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Documents_DocumentId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BillId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BillId",
                table: "Notifications",
                column: "BillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Bills_BillId",
                table: "Notifications",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "BillId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Documents_DocumentId",
                table: "Notifications",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Bills_BillId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Documents_DocumentId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_BillId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "BillId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Documents_DocumentId",
                table: "Notifications",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "DocumentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
