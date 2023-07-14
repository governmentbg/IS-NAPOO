using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAdd_ESignApplicationFileNameAllowNULL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ESignApplicationFileName",
                table: "Candidate_Provider",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Път до електронно подписанато заявление",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldComment: "Път до електронно подписанато заявление");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ESignApplicationFileName",
                table: "Candidate_Provider",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Път до електронно подписанато заявление",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "Път до електронно подписанато заявление");
        }
    }
}
