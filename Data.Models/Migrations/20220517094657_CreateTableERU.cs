using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableERU : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOC_ERU",
                columns: table => new
                {
                    IdERU = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdProfessionalTraining = table.Column<int>(type: "int", nullable: false),
                    IdNKRLevel = table.Column<int>(type: "int", nullable: false),
                    IdEKRLevel = table.Column<int>(type: "int", nullable: false),
                    RUText = table.Column<string>(type: "ntext", nullable: false),
                    IdDOC = table.Column<int>(type: "int", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOC_ERU", x => x.IdERU);
                    table.ForeignKey(
                        name: "FK_DOC_ERU_DOC_DOC_IdDOC",
                        column: x => x.IdDOC,
                        principalTable: "DOC_DOC",
                        principalColumn: "IdDOC",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOC_ERU_IdDOC",
                table: "DOC_ERU",
                column: "IdDOC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOC_ERU");
        }
    }
}
