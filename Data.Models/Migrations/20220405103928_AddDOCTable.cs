using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddDOCTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOC_DOC",
                columns: table => new
                {
                    IdDOC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdProfession = table.Column<int>(type: "int", nullable: false),
                    RequirementsCandidates = table.Column<string>(type: "ntext", nullable: false),
                    DescriptionProfession = table.Column<string>(type: "ntext", nullable: false),
                    RequirementsMaterialBase = table.Column<string>(type: "ntext", nullable: false),
                    RequirementsТrainers = table.Column<string>(type: "ntext", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOC_DOC", x => x.IdDOC);
                    table.ForeignKey(
                        name: "FK_DOC_DOC_SPPOO_Profession_IdProfession",
                        column: x => x.IdProfession,
                        principalTable: "SPPOO_Profession",
                        principalColumn: "IdProfession",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOC_DOC_IdProfession",
                table: "DOC_DOC",
                column: "IdProfession");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOC_DOC");
        }
    }
}
