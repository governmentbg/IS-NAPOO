using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderSpecialityAddLicenceProtNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LicenceProtNo",
                table: "Candidate_ProviderSpeciality",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Номер на протокол/заповед за лицензиране на специалността");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenceProtNo",
                table: "Candidate_ProviderSpeciality");
        }
    }
}
