using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableManagementDeadlineProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_TypeOfRequestedDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_TypeOfRequestedDocument",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_DocumentSeries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_DocumentSeries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Procedure_ManagementDeadlineProcedure",
                columns: table => new
                {
                    IdManagementDeadlineProcedure = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLicensingType = table.Column<int>(type: "int", nullable: false, comment: "Вид лицензия"),
                    IdApplicationStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус/Етап на процедурата по лицензиране"),
                    Term = table.Column<int>(type: "int", nullable: false, comment: "Срок"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_ManagementDeadlineProcedure", x => x.IdManagementDeadlineProcedure);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedure_ManagementDeadlineProcedure");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_TypeOfRequestedDocument");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_TypeOfRequestedDocument");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_DocumentSeries");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_DocumentSeries");
        }
    }
}
