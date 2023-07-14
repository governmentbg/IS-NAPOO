using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidate_Provider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProviderStatusId",
                table: "Provider",
                newName: "IdProviderStatus");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Provider",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdLocation",
                table: "Provider",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdProviderOwnership",
                table: "Provider",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdProviderRegistration",
                table: "Provider",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Candidate_Provider",
                columns: table => new
                {
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderOwner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PoviderBulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AttorneyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdProviderRegistration = table.Column<int>(type: "int", nullable: false),
                    IdProviderOwnership = table.Column<int>(type: "int", nullable: false),
                    IdProviderStatus = table.Column<int>(type: "int", nullable: false),
                    IdLocation = table.Column<int>(type: "int", nullable: true),
                    ProviderAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IdTypeLicense = table.Column<int>(type: "int", nullable: false),
                    ApplicationNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdLocationAdmin = table.Column<int>(type: "int", nullable: true),
                    ProviderAddressAdmin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ZipCodeAdmin = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    ProviderPhoneAdmin = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderFax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderWeb = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    AccessibilityInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    OnlineTrainingInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    PersonNameCorrespondence = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdLocationCorrespondence = table.Column<int>(type: "int", nullable: true),
                    ProviderAddressCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ZipCodeCorrespondence = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    ProviderPhoneCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderFaxCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderEmailCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_Provider", x => x.IdCandidate_Provider);
                    table.ForeignKey(
                        name: "FK_Candidate_Provider_Location_IdLocation",
                        column: x => x.IdLocation,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                    table.ForeignKey(
                        name: "FK_Candidate_Provider_Location_IdLocationAdmin",
                        column: x => x.IdLocationAdmin,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                    table.ForeignKey(
                        name: "FK_Candidate_Provider_Location_IdLocationCorrespondence",
                        column: x => x.IdLocationCorrespondence,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Provider_IdLocation",
                table: "Provider",
                column: "IdLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdLocation",
                table: "Candidate_Provider",
                column: "IdLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdLocationAdmin",
                table: "Candidate_Provider",
                column: "IdLocationAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdLocationCorrespondence",
                table: "Candidate_Provider",
                column: "IdLocationCorrespondence");

            migrationBuilder.AddForeignKey(
                name: "FK_Provider_Location_IdLocation",
                table: "Provider",
                column: "IdLocation",
                principalTable: "Location",
                principalColumn: "idLocation");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Provider_Location_IdLocation",
                table: "Provider");

            migrationBuilder.DropTable(
                name: "Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Provider_IdLocation",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "IdLocation",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "IdProviderOwnership",
                table: "Provider");

            migrationBuilder.DropColumn(
                name: "IdProviderRegistration",
                table: "Provider");

            migrationBuilder.RenameColumn(
                name: "IdProviderStatus",
                table: "Provider",
                newName: "ProviderStatusId");
        }
    }
}
