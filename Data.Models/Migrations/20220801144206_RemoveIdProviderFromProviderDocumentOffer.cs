using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class RemoveIdProviderFromProviderDocumentOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Provider_ProviderId",
                table: "Request_ProviderDocumentOffer");

            migrationBuilder.DropIndex(
                name: "IX_Request_ProviderDocumentOffer_ProviderId",
                table: "Request_ProviderDocumentOffer");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Request_ProviderDocumentOffer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "Request_ProviderDocumentOffer",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProviderDocumentOffer_ProviderId",
                table: "Request_ProviderDocumentOffer",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Provider_ProviderId",
                table: "Request_ProviderDocumentOffer",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id");
        }
    }
}
