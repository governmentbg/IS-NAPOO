using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableSelfAssessmentReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "Arch_SelfAssessmentReport",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Статус на доклад");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "Arch_SelfAssessmentReport");
        }
    }
}
