using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_CandidateProviderLicenceChange_CandidateProvider_AddColumn_Archive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Rating_Indicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.AlterColumn<int>(
                name: "IdIndicator",
                table: "Rating_CandidateProviderIndicator",
                type: "int",
                nullable: true,
                comment: "Връзка с Показател",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Връзка с Показател");

            migrationBuilder.AddColumn<string>(
                name: "Archive",
                table: "Candidate_ProviderLicenceChange",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Информация къде се съхранява архивът на ЦПО/ЦИПО при отнемане на лицензия");

            migrationBuilder.AddColumn<string>(
                name: "Archive",
                table: "Candidate_Provider",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Информация къде се съхранява архивът на ЦПО/ЦИПО при отнемане на лицензия");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Rating_Indicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator",
                column: "IdIndicator",
                principalTable: "Rating_Indicator",
                principalColumn: "IdIndicator");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Rating_Indicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropColumn(
                name: "Archive",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "Archive",
                table: "Candidate_Provider");

            migrationBuilder.AlterColumn<int>(
                name: "IdIndicator",
                table: "Rating_CandidateProviderIndicator",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Връзка с Показател",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Връзка с Показател");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Rating_Indicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator",
                column: "IdIndicator",
                principalTable: "Rating_Indicator",
                principalColumn: "IdIndicator",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
