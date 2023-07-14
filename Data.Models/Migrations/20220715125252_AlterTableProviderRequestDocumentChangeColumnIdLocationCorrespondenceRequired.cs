using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProviderRequestDocumentChangeColumnIdLocationCorrespondenceRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderRequestDocument_Location_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.AlterColumn<int>(
                name: "IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Населено място за кореспондениця на ЦПО,ЦИПО",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Населено място за кореспондениця на ЦПО,ЦИПО");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderRequestDocument_Location_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument",
                column: "IdLocationCorrespondence",
                principalTable: "Location",
                principalColumn: "idLocation",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderRequestDocument_Location_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.AlterColumn<int>(
                name: "IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: true,
                comment: "Населено място за кореспондениця на ЦПО,ЦИПО",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Населено място за кореспондениця на ЦПО,ЦИПО");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderRequestDocument_Location_IdLocationCorrespondence",
                table: "Request_ProviderRequestDocument",
                column: "IdLocationCorrespondence",
                principalTable: "Location",
                principalColumn: "idLocation");
        }
    }
}
