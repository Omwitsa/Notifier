using Microsoft.EntityFrameworkCore.Migrations;

namespace AbnNotifier.Migrations
{
    public partial class StoreRecipientOnSent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SentNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "SentNotifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "SentNotifications");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "SentNotifications");
        }
    }
}
