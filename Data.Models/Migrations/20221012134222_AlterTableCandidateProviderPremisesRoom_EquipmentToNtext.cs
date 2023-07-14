using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderPremisesRoom_EquipmentToNtext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Equipment",
                table: "Candidate_ProviderPremisesRoom",
                type: "ntext",
                nullable: true,
                comment: "Кратко описание на оборудването",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Equipment",
                table: "Candidate_ProviderPremisesRoom",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Кратко описание на оборудването");
        }
    }
}
