using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddIdLicenceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdDocumentStatus",
                table: "Training_ClientCourseDocument",
                type: "int",
                nullable: true,
                comment: "Статус на документ за завършено обучение",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Статус");

            migrationBuilder.AddColumn<int>(
                name: "IdLicenceStatus",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                comment: "Статус на  лицензията");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdLicenceStatus",
                table: "Candidate_Provider");

            migrationBuilder.AlterColumn<int>(
                name: "IdDocumentStatus",
                table: "Training_ClientCourseDocument",
                type: "int",
                nullable: true,
                comment: "Статус",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Статус на документ за завършено обучение");
        }
    }
}
