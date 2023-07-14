using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTableFrameworkProgram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SPPOO_FrameworkProgram",
                columns: table => new
                {
                    IdFrameworkProgram = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IdVQS = table.Column<int>(type: "int", nullable: false),
                    IdTypeFrameworkProgram = table.Column<int>(type: "int", nullable: false),
                    IdQualificationLevel = table.Column<int>(type: "int", nullable: false),
                    IdFormEducation = table.Column<int>(type: "int", nullable: false),
                    IdMinimumLevelEducation = table.Column<int>(type: "int", nullable: false),
                    IdMinimumQualificationLevel = table.Column<int>(type: "int", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_FrameworkProgram", x => x.IdFrameworkProgram);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPPOO_FrameworkProgram");
        }
    }
}
