using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableExpertDOC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpComm_ExpertDOC",
                columns: table => new
                {
                    IdExpertDOC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdExpert = table.Column<int>(type: "int", nullable: false, comment: "Експерт"),
                    IdDOC = table.Column<int>(type: "int", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Номер на заповед"),
                    DateOrder = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на утвърждаване"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpComm_ExpertDOC", x => x.IdExpertDOC);
                    table.ForeignKey(
                        name: "FK_ExpComm_ExpertDOC_DOC_DOC_IdDOC",
                        column: x => x.IdDOC,
                        principalTable: "DOC_DOC",
                        principalColumn: "IdDOC",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExpComm_ExpertDOC_ExpComm_Expert_IdExpert",
                        column: x => x.IdExpert,
                        principalTable: "ExpComm_Expert",
                        principalColumn: "IdExpert",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_ExpertDOC_IdDOC",
                table: "ExpComm_ExpertDOC",
                column: "IdDOC");

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_ExpertDOC_IdExpert",
                table: "ExpComm_ExpertDOC",
                column: "IdExpert");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpComm_ExpertDOC");
        }
    }
}
