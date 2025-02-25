using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Integrations.Migrations
{
    /// <inheritdoc />
    public partial class AddSecretToTicketsTr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReject",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReject",
                table: "Tickets");
        }
    }
}
