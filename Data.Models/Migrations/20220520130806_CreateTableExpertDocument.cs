using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableExpertDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpComm_ExpertDocument",
                columns: table => new
                {
                    IdExpertDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdExpert = table.Column<int>(type: "int", nullable: false),
                    IdDocumentType = table.Column<int>(type: "int", nullable: false),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpComm_ExpertDocument", x => x.IdExpertDocument);
                    table.ForeignKey(
                        name: "FK_ExpComm_ExpertDocument_ExpComm_Expert_IdExpert",
                        column: x => x.IdExpert,
                        principalTable: "ExpComm_Expert",
                        principalColumn: "IdExpert",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_ExpertDocument_IdExpert",
                table: "ExpComm_ExpertDocument",
                column: "IdExpert");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpComm_ExpertDocument");
        }
    }
}
