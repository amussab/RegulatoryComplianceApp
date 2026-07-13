using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegulatoryComplianceApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTypeManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DocumentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExpirable",
                table: "DocumentTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId1",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeId1",
                table: "Documents",
                column: "DocumentTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId1",
                table: "Documents",
                column: "DocumentTypeId1",
                principalTable: "DocumentTypes",
                principalColumn: "DocumentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_DocumentTypes_DocumentTypeId1",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_DocumentTypeId1",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "IsExpirable",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId1",
                table: "Documents");
        }
    }
}
