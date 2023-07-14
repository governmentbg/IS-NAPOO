using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class alterTableAddIdExpertToTable_ExpertProfessionalDirection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdExpert",
                table: "ExpComm_ExpertProfessionalDirection",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_ExpertProfessionalDirection_IdExpert",
                table: "ExpComm_ExpertProfessionalDirection",
                column: "IdExpert");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpComm_ExpertProfessionalDirection_ExpComm_Expert_IdExpert",
                table: "ExpComm_ExpertProfessionalDirection",
                column: "IdExpert",
                principalTable: "ExpComm_Expert",
                principalColumn: "IdExpert",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpComm_ExpertProfessionalDirection_ExpComm_Expert_IdExpert",
                table: "ExpComm_ExpertProfessionalDirection");

            migrationBuilder.DropIndex(
                name: "IX_ExpComm_ExpertProfessionalDirection_IdExpert",
                table: "ExpComm_ExpertProfessionalDirection");

            migrationBuilder.DropColumn(
                name: "IdExpert",
                table: "ExpComm_ExpertProfessionalDirection");
        }
    }
}
