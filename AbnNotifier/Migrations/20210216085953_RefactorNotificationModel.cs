using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AbnNotifier.Migrations
{
    public partial class RefactorNotificationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SentNotifications");

            migrationBuilder.DropTable(
                name: "Supervisors");

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DocNo = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    IsFinalStatus = table.Column<bool>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    Department = table.Column<string>(nullable: true),
                    Empno = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: true),
                    DateModified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Approver",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    NotificationId = table.Column<Guid>(nullable: true),
                    EmpNo = table.Column<string>(nullable: true),
                    UserCode = table.Column<string>(nullable: true),
                    Level = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Approver_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Approver_NotificationId",
                table: "Approver",
                column: "NotificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Approver");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.CreateTable(
                name: "SentNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Department = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EmpNo = table.Column<string>(nullable: true),
                    Event = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(nullable: true),
                    Response = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Success = table.Column<bool>(nullable: false),
                    Supervisor = table.Column<string>(nullable: true),
                    UniIntId = table.Column<int>(nullable: false),
                    UniStrId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Department = table.Column<int>(nullable: false),
                    EmpNo = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.Id);
                });
        }
    }
}
