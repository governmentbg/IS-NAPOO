using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_FollowUpControlDocumentNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Control_FollowUpControlDocumentNotification",
                columns: table => new
                {
                    IdFollowUpControlDocumentNotification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFollowUpControlDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с документ по процедура"),
                    IdNotification = table.Column<int>(type: "int", nullable: false, comment: "Връзка с известие"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control_FollowUpControlDocumentNotification", x => x.IdFollowUpControlDocumentNotification);
                    table.ForeignKey(
                        name: "FK_Control_FollowUpControlDocumentNotification_Control_FollowUpControlDocument_IdFollowUpControlDocument",
                        column: x => x.IdFollowUpControlDocument,
                        principalTable: "Control_FollowUpControlDocument",
                        principalColumn: "IdFollowUpControlDocument",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Control_FollowUpControlDocumentNotification_Notification_IdNotification",
                        column: x => x.IdNotification,
                        principalTable: "Notification",
                        principalColumn: "IdNotification",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Control_FollowUpControlDocumentNotification_IdFollowUpControlDocument",
                table: "Control_FollowUpControlDocumentNotification",
                column: "IdFollowUpControlDocument");

            migrationBuilder.CreateIndex(
                name: "IX_Control_FollowUpControlDocumentNotification_IdNotification",
                table: "Control_FollowUpControlDocumentNotification",
                column: "IdNotification");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Control_FollowUpControlDocumentNotification");
        }
    }
}
