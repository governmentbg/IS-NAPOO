using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAdd_ESignApplicationFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderActive",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                comment: "Връзка с активния канидат провайдър",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ESignApplicationFileName",
                table: "Candidate_Provider",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Път до електронно подписанато заявление");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ESignApplicationFileName",
                table: "Candidate_Provider");

            migrationBuilder.AlterColumn<int>(
                name: "IdCandidateProviderActive",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Връзка с активния канидат провайдър");
        }
    }
}
