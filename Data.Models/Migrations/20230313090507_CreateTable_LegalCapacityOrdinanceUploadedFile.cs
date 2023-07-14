using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_LegalCapacityOrdinanceUploadedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SPPOO_LegalCapacityOrdinanceUploadedFile",
                columns: table => new
                {
                    IdLegalCapacityOrdinanceUploadedFile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLegalCapacityOrdinanceType = table.Column<int>(type: "int", nullable: false, comment: "Вид на наредбата за правоспособност"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_LegalCapacityOrdinanceUploadedFile", x => x.IdLegalCapacityOrdinanceUploadedFile);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPPOO_LegalCapacityOrdinanceUploadedFile");
        }
    }
}
