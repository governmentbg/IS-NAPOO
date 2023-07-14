using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class DropProcedureDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedure_Document");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procedure_Document",
                columns: table => new
                {
                    IdStartedProcedureProgress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DS_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_ID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_LINK = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DS_OFFICIAL_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_OFFICIAL_ID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DS_PREP = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DateAttachment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    IdDocumentType = table.Column<int>(type: "int", nullable: true),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UIN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_Document", x => x.IdStartedProcedureProgress);
                    table.ForeignKey(
                        name: "FK_Procedure_Document_Procedure_StartedProcedure_IdStartedProcedure",
                        column: x => x.IdStartedProcedure,
                        principalTable: "Procedure_StartedProcedure",
                        principalColumn: "IdStartedProcedure",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_Document_IdStartedProcedure",
                table: "Procedure_Document",
                column: "IdStartedProcedure");
        }
    }
}
