using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProcedureExternalExpertAddColumnIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Procedure_ExternalExpert",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Показва статуса на външния експерт спрямо процедурата");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Procedure_ExternalExpert");
        }
    }
}
