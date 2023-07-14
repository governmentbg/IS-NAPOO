using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProcedurePrice_add_AdditionalInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdditionalInformation",
                table: "Procedure_ProcedurePrice",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Допълнителна информация");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInformation",
                table: "Procedure_ProcedurePrice");
        }
    }
}
