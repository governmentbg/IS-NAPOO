using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddFieldsForLatinic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonNameCorrespondenceEN",
                table: "Candidate_Provider",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Лице за контакт/кореспондениця на латиница");

            migrationBuilder.AddColumn<string>(
                name: "ProviderAddressCorrespondenceEN",
                table: "Candidate_Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Адрес за кореспонденция   на ЦПО,ЦИПО на латиница");

            migrationBuilder.AddColumn<string>(
                name: "ProviderNameEN",
                table: "Candidate_Provider",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Име на ЦПО,ЦИПО на Латиница");

            migrationBuilder.AddColumn<string>(
                name: "ProviderOwnerEN",
                table: "Candidate_Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                comment: "Наименование на юридическото лице на латиница");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonNameCorrespondenceEN",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "ProviderAddressCorrespondenceEN",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "ProviderNameEN",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "ProviderOwnerEN",
                table: "Candidate_Provider");
        }
    }
}
