using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    IdNotification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    About = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NotificationText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IdPersonFrom = table.Column<int>(type: "int", nullable: true),
                    IdPersonTo = table.Column<int>(type: "int", nullable: true),
                    IdStatusNotification = table.Column<int>(type: "int", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.IdNotification);
                    table.ForeignKey(
                        name: "FK_Notification_Person_IdPersonFrom",
                        column: x => x.IdPersonFrom,
                        principalTable: "Person",
                        principalColumn: "IdPerson");
                    table.ForeignKey(
                        name: "FK_Notification_Person_IdPersonTo",
                        column: x => x.IdPersonTo,
                        principalTable: "Person",
                        principalColumn: "IdPerson");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_IdPersonFrom",
                table: "Notification",
                column: "IdPersonFrom");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_IdPersonTo",
                table: "Notification",
                column: "IdPersonTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notification");
        }
    }
}
