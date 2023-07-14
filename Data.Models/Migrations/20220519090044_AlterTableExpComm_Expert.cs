using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableExpComm_Expert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ExpComm_Expert",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ExpComm_Expert",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdLocation",
                table: "ExpComm_Expert",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ExpComm_Expert",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "ExpComm_Expert",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_Expert_IdLocation",
                table: "ExpComm_Expert",
                column: "IdLocation");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpComm_Expert_Location_IdLocation",
                table: "ExpComm_Expert",
                column: "IdLocation",
                principalTable: "Location",
                principalColumn: "idLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpComm_Expert_Location_IdLocation",
                table: "ExpComm_Expert");

            migrationBuilder.DropIndex(
                name: "IX_ExpComm_Expert_IdLocation",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "IdLocation",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "ExpComm_Expert");
        }
    }
}
