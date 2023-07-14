using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProviderAddColumnsForCorrespondenceAndRenamePhoneColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderPhone2",
                table: "Provider",
                newName: "ProviderPhoneCorrespondence");

            migrationBuilder.RenameColumn(
                name: "ProviderPhone1",
                table: "Provider",
                newName: "ProviderPhone");

            migrationBuilder.RenameColumn(
                name: "ProviderPhoneAdmin",
                table: "Candidate_Provider",
                newName: "ProviderPhone");

            migrationBuilder.AddColumn<int>(
                name: "IdLocationCorrespondence",
                table: "Provider",
                type: "int",
                nullable: true,
                comment: "Населено място за кореспондениця на ЦПО,ЦИПО");

            migrationBuilder.AddColumn<string>(
                name: "PersonNameCorrespondence",
                table: "Provider",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Лице за контакт/кореспондениця");

            migrationBuilder.AddColumn<string>(
                name: "ProviderAddressCorrespondence",
                table: "Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Адрес за кореспонденция   на ЦПО,ЦИПО");

            migrationBuilder.AddColumn<string>(
                name: "ProviderEmailCorrespondence",
                table: "Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "E-mail за кореспонденция с ЦПО,ЦИПО");

            migrationBuilder.AddColumn<string>(
                name: "ProviderFaxCorrespondence",
                table: "Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Факс за кореспонденция с ЦПО,ЦИПО");

            migrationBuilder.AddColumn<string>(
                name: "ZipCodeCorrespondence",
                table: "Provider",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true,
                comment: "Пощенски код за кореспонденция   на ЦПО,ЦИПО");

            migrationBuilder.CreateIndex(
                name: "IX_Provider_IdLocationCorrespondence",
                table: "Provider",
                column: "IdLocationCorrespondence");

            migrationBuilder.AddForeignKey(
                name: "FK_Provider_Location_IdLocationCorrespondence",
                table: "Provider",
                column: "IdLocationCorrespondence",
                principalTable: "Location",
                principalColumn: "idLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Provider_Location_IdLocationCorrespondence",
                table: "Provider");

            migrationBuilder.DropIndex(
                name: "IX_Provider_IdLocationCorrespondence",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "IdLocationCorrespondence",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "PersonNameCorrespondence",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "ProviderAddressCorrespondence",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "ProviderEmailCorrespondence",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "ProviderFaxCorrespondence",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "ZipCodeCorrespondence",
                table: "Provider");

            migrationBuilder.RenameColumn(
                name: "ProviderPhoneCorrespondence",
                table: "Provider",
                newName: "ProviderPhone2");

            migrationBuilder.RenameColumn(
                name: "ProviderPhone",
                table: "Provider",
                newName: "ProviderPhone1");

            migrationBuilder.RenameColumn(
                name: "ProviderPhone",
                table: "Candidate_Provider",
                newName: "ProviderPhoneAdmin");
        }
    }
}
