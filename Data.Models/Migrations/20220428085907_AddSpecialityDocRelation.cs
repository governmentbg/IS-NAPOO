using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddSpecialityDocRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdDOC",
                table: "SPPOO_Speciality",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_Speciality_IdDOC",
                table: "SPPOO_Speciality",
                column: "IdDOC");

            migrationBuilder.AddForeignKey(
                name: "FK_SPPOO_Speciality_DOC_DOC_IdDOC",
                table: "SPPOO_Speciality",
                column: "IdDOC",
                principalTable: "DOC_DOC",
                principalColumn: "IdDOC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SPPOO_Speciality_DOC_DOC_IdDOC",
                table: "SPPOO_Speciality");

            migrationBuilder.DropIndex(
                name: "IX_SPPOO_Speciality_IdDOC",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "IdDOC",
                table: "SPPOO_Speciality");
        }
    }
}
