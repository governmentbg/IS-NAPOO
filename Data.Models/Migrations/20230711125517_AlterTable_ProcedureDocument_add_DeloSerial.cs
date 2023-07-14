using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ProcedureDocument_add_DeloSerial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeloSerial",
                table: "Procedure_Document",
                type: "int",
                nullable: true,
                comment: "Пореден номер на документ в преписката, в която е създаден");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeloSerial",
                table: "Procedure_Document");
        }
    }
}
