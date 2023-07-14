using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableExpertCommissionOccupationLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Occupation",
                table: "ExpComm_ExpertExpertCommission",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                comment: "Длъжност ",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Длъжност ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Occupation",
                table: "ExpComm_ExpertExpertCommission",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Длъжност ",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldComment: "Длъжност ");
        }
    }
}
