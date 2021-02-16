using Microsoft.EntityFrameworkCore.Migrations;

namespace AbnNotifier.Migrations
{
    public partial class UpdateSentNots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UniId",
                table: "SentNotifications",
                newName: "UniIntId");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "SentNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "SentNotifications",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "SentNotifications",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Supervisor",
                table: "SentNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UniStrId",
                table: "SentNotifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "SentNotifications");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "SentNotifications");

            migrationBuilder.DropColumn(
                name: "Success",
                table: "SentNotifications");

            migrationBuilder.DropColumn(
                name: "Supervisor",
                table: "SentNotifications");

            migrationBuilder.DropColumn(
                name: "UniStrId",
                table: "SentNotifications");

            migrationBuilder.RenameColumn(
                name: "UniIntId",
                table: "SentNotifications",
                newName: "UniId");
        }
    }
}
