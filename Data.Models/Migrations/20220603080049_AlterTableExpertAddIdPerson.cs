using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableExpertAddIdPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdPerson",
                table: "ExpComm_Expert",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_Expert_IdPerson",
                table: "ExpComm_Expert",
                column: "IdPerson");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpComm_Expert_Person_IdPerson",
                table: "ExpComm_Expert",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "IdPerson");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpComm_Expert_Person_IdPerson",
                table: "ExpComm_Expert");

            migrationBuilder.DropIndex(
                name: "IX_ExpComm_Expert_IdPerson",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "IdPerson",
                table: "ExpComm_Expert");
        }
    }
}
