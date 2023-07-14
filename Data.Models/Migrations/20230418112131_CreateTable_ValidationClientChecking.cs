using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_ValidationClientChecking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ValidationClientChecking",
                columns: table => new
                {
                    IdValidationClientChecking = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с валидирано лице"),
                    IdFollowUpControl = table.Column<int>(type: "int", nullable: true, comment: "Последващ контрол, изпълняван от служител/и на НАПОО"),
                    CheckDone = table.Column<bool>(type: "bit", nullable: false, comment: "Извършена проверка от експерт на НАПОО"),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Коментар"),
                    CheckingDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на проверка"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationClientChecking", x => x.IdValidationClientChecking);
                    table.ForeignKey(
                        name: "FK_Training_ValidationClientChecking_Control_FollowUpControl_IdFollowUpControl",
                        column: x => x.IdFollowUpControl,
                        principalTable: "Control_FollowUpControl",
                        principalColumn: "IdFollowUpControl");
                    table.ForeignKey(
                        name: "FK_Training_ValidationClientChecking_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientChecking_IdFollowUpControl",
                table: "Training_ValidationClientChecking",
                column: "IdFollowUpControl");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientChecking_IdValidationClient",
                table: "Training_ValidationClientChecking",
                column: "IdValidationClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationClientChecking");
        }
    }
}
