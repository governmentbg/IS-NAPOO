using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableProcedureDocumentNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procedure_DocumentNotification",
                columns: table => new
                {
                    IdProcedureDocumentNotification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProcedureDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с документ по процедура"),
                    IdNotification = table.Column<int>(type: "int", nullable: false, comment: "Връзка с известие"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_DocumentNotification", x => x.IdProcedureDocumentNotification);
                    table.ForeignKey(
                        name: "FK_Procedure_DocumentNotification_Notification_IdNotification",
                        column: x => x.IdNotification,
                        principalTable: "Notification",
                        principalColumn: "IdNotification",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedure_DocumentNotification_Procedure_Document_IdProcedureDocument",
                        column: x => x.IdProcedureDocument,
                        principalTable: "Procedure_Document",
                        principalColumn: "IdProcedureDocument",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_DocumentNotification_IdNotification",
                table: "Procedure_DocumentNotification",
                column: "IdNotification");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_DocumentNotification_IdProcedureDocument",
                table: "Procedure_DocumentNotification",
                column: "IdProcedureDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedure_DocumentNotification");
        }
    }
}
