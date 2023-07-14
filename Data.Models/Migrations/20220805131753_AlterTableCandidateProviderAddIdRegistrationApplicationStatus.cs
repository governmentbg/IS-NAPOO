using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddIdRegistrationApplicationStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdApplicationStatus",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                comment: "Статус на  заявлението",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdRegistrationApplicationStatus",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                comment: "Статус на регистрация на заявлението");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdRegistrationApplicationStatus",
                table: "Candidate_Provider");

            migrationBuilder.AlterColumn<int>(
                name: "IdApplicationStatus",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Статус на  заявлението");
        }
    }
}
