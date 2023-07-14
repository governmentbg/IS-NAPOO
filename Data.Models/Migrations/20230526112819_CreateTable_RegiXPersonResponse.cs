using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_RegiXPersonResponse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_RegiXPersonResponse",
                columns: table => new
                {
                    IdRegiXPersonResponse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EGN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false, comment: "Стойност на ЕГН"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Име"),
                    SecondName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Презиме"),
                    FamilyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Фамилия"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на раждане"),
                    DeathDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на смърт"),
                    CheckDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на проверката в RegiX"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_RegiXPersonResponse", x => x.IdRegiXPersonResponse);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_RegiXPersonResponse");
        }
    }
}
