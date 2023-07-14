using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProviderRequestDocumentAddColumnIdLocationCorrespondence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: true,
                comment: "Населено място за кореспондениця на ЦПО,ЦИПО");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProviderRequestDocument_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument",
                column: "IdLocationCorrespondence");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderRequestDocument_Location_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument",
                column: "IdLocationCorrespondence",
                principalTable: "Location",
                principalColumn: "idLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderRequestDocument_Location_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.DropIndex(
                name: "IX_Request_ProviderRequestDocument_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.DropColumn(
                name: "IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument");
        }
    }
}
