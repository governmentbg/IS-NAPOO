using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTableNKPD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOC_NKPD",
                columns: table => new
                {
                    IdNKPD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IdClassCode = table.Column<int>(type: "int", nullable: false),
                    IdSubclassCode = table.Column<int>(type: "int", nullable: false),
                    IdGroupCode = table.Column<int>(type: "int", nullable: false),
                    IdIndividualGroupCode = table.Column<int>(type: "int", nullable: false),
                    EducationLevelCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOC_NKPD", x => x.IdNKPD);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOC_NKPD");
        }
    }
}
