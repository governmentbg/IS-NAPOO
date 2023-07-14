using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ExpertExpertCommission_add_IdMemberType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdMemberType",
                table: "ExpComm_ExpertExpertCommission",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Тип на участника");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdMemberType",
                table: "ExpComm_ExpertExpertCommission");
        }
    }
}
