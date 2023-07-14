using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableChangeProviderToCandidateProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_DocumentSerialNumber_Provider_IdProvider",
                table: "Request_DocumentSerialNumber");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Provider_IdProvider",
                table: "Request_ProviderDocumentOffer");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderRequestDocument_Provider_IdProvider",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentManagement_Provider_IdProvider",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentManagement_Provider_IdProviderPartner",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentStatus_Provider_IdProvider",
                table: "Request_RequestDocumentStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentType_Provider_IdProvider",
                table: "Request_RequestDocumentType");

            migrationBuilder.RenameColumn(
                name: "IdProvider",
                table: "Request_RequestDocumentType",
                newName: "IdCandidateProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentType_IdProvider",
                table: "Request_RequestDocumentType",
                newName: "IX_Request_RequestDocumentType_IdCandidateProvider");

            migrationBuilder.RenameColumn(
                name: "IdProvider",
                table: "Request_RequestDocumentStatus",
                newName: "IdCandidateProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentStatus_IdProvider",
                table: "Request_RequestDocumentStatus",
                newName: "IX_Request_RequestDocumentStatus_IdCandidateProvider");

            migrationBuilder.RenameColumn(
                name: "IdProviderPartner",
                table: "Request_RequestDocumentManagement",
                newName: "IdCandidateProviderPartner");

            migrationBuilder.RenameColumn(
                name: "IdProvider",
                table: "Request_RequestDocumentManagement",
                newName: "IdCandidateProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentManagement_IdProviderPartner",
                table: "Request_RequestDocumentManagement",
                newName: "IX_Request_RequestDocumentManagement_IdCandidateProviderPartner");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentManagement_IdProvider",
                table: "Request_RequestDocumentManagement",
                newName: "IX_Request_RequestDocumentManagement_IdCandidateProvider");

            migrationBuilder.RenameColumn(
                name: "IdProvider",
                table: "Request_ProviderRequestDocument",
                newName: "IdCandidateProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_ProviderRequestDocument_IdProvider",
                table: "Request_ProviderRequestDocument",
                newName: "IX_Request_ProviderRequestDocument_IdCandidateProvider");

            migrationBuilder.RenameColumn(
                name: "IdProvider",
                table: "Request_ProviderDocumentOffer",
                newName: "IdCandidateProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_ProviderDocumentOffer_IdProvider",
                table: "Request_ProviderDocumentOffer",
                newName: "IX_Request_ProviderDocumentOffer_IdCandidateProvider");

            migrationBuilder.RenameColumn(
                name: "IdProvider",
                table: "Request_DocumentSerialNumber",
                newName: "IdCandidateProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_DocumentSerialNumber_IdProvider",
                table: "Request_DocumentSerialNumber",
                newName: "IX_Request_DocumentSerialNumber_IdCandidateProvider");

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
                name: "FK_Request_DocumentSerialNumber_Candidate_Provider_IdCandidateProvider",
                table: "Request_DocumentSerialNumber",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Candidate_Provider_IdCandidateProvider",
                table: "Request_ProviderDocumentOffer",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Provider_ProviderId",
                table: "Request_ProviderDocumentOffer",
                column: "ProviderId",
                principalTable: "Provider",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderRequestDocument_Candidate_Provider_IdCandidateProvider",
                table: "Request_ProviderRequestDocument",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentManagement_Candidate_Provider_IdCandidateProvider",
                table: "Request_RequestDocumentManagement",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentManagement_Candidate_Provider_IdCandidateProviderPartner",
                table: "Request_RequestDocumentManagement",
                column: "IdCandidateProviderPartner",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentStatus_Candidate_Provider_IdCandidateProvider",
                table: "Request_RequestDocumentStatus",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentType_Candidate_Provider_IdCandidateProvider",
                table: "Request_RequestDocumentType",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_DocumentSerialNumber_Candidate_Provider_IdCandidateProvider",
                table: "Request_DocumentSerialNumber");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Candidate_Provider_IdCandidateProvider",
                table: "Request_ProviderDocumentOffer");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Provider_ProviderId",
                table: "Request_ProviderDocumentOffer");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_ProviderRequestDocument_Candidate_Provider_IdCandidateProvider",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentManagement_Candidate_Provider_IdCandidateProvider",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentManagement_Candidate_Provider_IdCandidateProviderPartner",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentStatus_Candidate_Provider_IdCandidateProvider",
                table: "Request_RequestDocumentStatus");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentType_Candidate_Provider_IdCandidateProvider",
                table: "Request_RequestDocumentType");

            migrationBuilder.DropIndex(
                name: "IX_Request_ProviderDocumentOffer_ProviderId",
                table: "Request_ProviderDocumentOffer");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "Request_ProviderDocumentOffer");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProvider",
                table: "Request_RequestDocumentType",
                newName: "IdProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentType_IdCandidateProvider",
                table: "Request_RequestDocumentType",
                newName: "IX_Request_RequestDocumentType_IdProvider");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProvider",
                table: "Request_RequestDocumentStatus",
                newName: "IdProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentStatus_IdCandidateProvider",
                table: "Request_RequestDocumentStatus",
                newName: "IX_Request_RequestDocumentStatus_IdProvider");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProviderPartner",
                table: "Request_RequestDocumentManagement",
                newName: "IdProviderPartner");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProvider",
                table: "Request_RequestDocumentManagement",
                newName: "IdProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentManagement_IdCandidateProviderPartner",
                table: "Request_RequestDocumentManagement",
                newName: "IX_Request_RequestDocumentManagement_IdProviderPartner");

            migrationBuilder.RenameIndex(
                name: "IX_Request_RequestDocumentManagement_IdCandidateProvider",
                table: "Request_RequestDocumentManagement",
                newName: "IX_Request_RequestDocumentManagement_IdProvider");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProvider",
                table: "Request_ProviderRequestDocument",
                newName: "IdProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_ProviderRequestDocument_IdCandidateProvider",
                table: "Request_ProviderRequestDocument",
                newName: "IX_Request_ProviderRequestDocument_IdProvider");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProvider",
                table: "Request_ProviderDocumentOffer",
                newName: "IdProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_ProviderDocumentOffer_IdCandidateProvider",
                table: "Request_ProviderDocumentOffer",
                newName: "IX_Request_ProviderDocumentOffer_IdProvider");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProvider",
                table: "Request_DocumentSerialNumber",
                newName: "IdProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Request_DocumentSerialNumber_IdCandidateProvider",
                table: "Request_DocumentSerialNumber",
                newName: "IX_Request_DocumentSerialNumber_IdProvider");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_DocumentSerialNumber_Provider_IdProvider",
                table: "Request_DocumentSerialNumber",
                column: "IdProvider",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderDocumentOffer_Provider_IdProvider",
                table: "Request_ProviderDocumentOffer",
                column: "IdProvider",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_ProviderRequestDocument_Provider_IdProvider",
                table: "Request_ProviderRequestDocument",
                column: "IdProvider",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentManagement_Provider_IdProvider",
                table: "Request_RequestDocumentManagement",
                column: "IdProvider",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentManagement_Provider_IdProviderPartner",
                table: "Request_RequestDocumentManagement",
                column: "IdProviderPartner",
                principalTable: "Provider",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentStatus_Provider_IdProvider",
                table: "Request_RequestDocumentStatus",
                column: "IdProvider",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentType_Provider_IdProvider",
                table: "Request_RequestDocumentType",
                column: "IdProvider",
                principalTable: "Provider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
