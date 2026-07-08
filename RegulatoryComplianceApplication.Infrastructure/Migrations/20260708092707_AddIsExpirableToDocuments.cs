using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegulatoryComplianceApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsExpirableToDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExpirable",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExpirable",
                table: "Documents");
        }
    }
}
