using Microsoft.EntityFrameworkCore.Migrations;

namespace AbnNotifier.Migrations
{
    public partial class AddSetupFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanSend",
                table: "EmailSmsSettings",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanSend",
                table: "EmailSmsSettings");
        }
    }
}
