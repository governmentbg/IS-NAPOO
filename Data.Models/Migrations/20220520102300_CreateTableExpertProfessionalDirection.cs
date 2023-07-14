using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableExpertProfessionalDirection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "DOC_DOC",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ExpComm_ExpertProfessionalDirection",
                columns: table => new
                {
                    IdExpertProfessionalDirection = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdExpertType = table.Column<int>(type: "int", nullable: false),
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DateApprovalExternalExpert = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DateOrderIncludedExpertCommission = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpComm_ExpertProfessionalDirection", x => x.IdExpertProfessionalDirection);
                    table.ForeignKey(
                        name: "FK_ExpComm_ExpertProfessionalDirection_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                        column: x => x.IdProfessionalDirection,
                        principalTable: "SPPOO_ProfessionalDirection",
                        principalColumn: "IdProfessionalDirection",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_ExpertProfessionalDirection_IdProfessionalDirection",
                table: "ExpComm_ExpertProfessionalDirection",
                column: "IdProfessionalDirection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpComm_ExpertProfessionalDirection");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "DOC_DOC");
        }
    }
}
