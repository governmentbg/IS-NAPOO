using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderPremisesRoomAddIDataMigrationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Procedure_ProcedurePrice",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInformation",
                table: "Procedure_ProcedurePrice",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Допълнителна информация",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Допълнителна информация");

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderPremisesRoom",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderPremisesRoom",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderPremisesRoom");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderPremisesRoom");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Procedure_ProcedurePrice",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInformation",
                table: "Procedure_ProcedurePrice",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Допълнителна информация",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Допълнителна информация");
        }
    }
}
