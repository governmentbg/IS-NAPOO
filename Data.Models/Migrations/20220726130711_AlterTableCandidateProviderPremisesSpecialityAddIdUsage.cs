using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderPremisesSpecialityAddIdUsage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Provider_Location_IdLocationAdmin",
                table: "Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_Provider_IdLocationAdmin",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdLocationAdmin",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "ProviderAddressAdmin",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "ZipCodeAdmin",
                table: "Candidate_Provider");

            migrationBuilder.AddColumn<int>(
                name: "IdUsage",
                table: "Candidate_ProviderPremisesSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Вид на провежданото обучение");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdUsage",
                table: "Candidate_ProviderPremisesSpeciality");

            migrationBuilder.AddColumn<int>(
                name: "IdLocationAdmin",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderAddressAdmin",
                table: "Candidate_Provider",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCodeAdmin",
                table: "Candidate_Provider",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdLocationAdmin",
                table: "Candidate_Provider",
                column: "IdLocationAdmin");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Provider_Location_IdLocationAdmin",
                table: "Candidate_Provider",
                column: "IdLocationAdmin",
                principalTable: "Location",
                principalColumn: "idLocation");
        }
    }
}
