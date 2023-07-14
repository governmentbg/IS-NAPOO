using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ValidationClient_AddIdCityOfBirthForeignKeyToLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClient_IdCityOfBirth",
                table: "Training_ValidationClient",
                column: "IdCityOfBirth");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ValidationClient_Location_IdCityOfBirth",
                table: "Training_ValidationClient",
                column: "IdCityOfBirth",
                principalTable: "Location",
                principalColumn: "idLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ValidationClient_Location_IdCityOfBirth",
                table: "Training_ValidationClient");

            migrationBuilder.DropIndex(
                name: "IX_Training_ValidationClient_IdCityOfBirth",
                table: "Training_ValidationClient");
        }
    }
}
