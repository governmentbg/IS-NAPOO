using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTablesTypeOfRequestedDocumentAndRequestDocumentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_TypeOfRequestedDocument",
                columns: table => new
                {
                    IdTypeOfRequestedDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocTypeOfficialNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Официален номер на документ"),
                    DocTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Наименование на документ"),
                    IsValid = table.Column<bool>(type: "bit", nullable: false, comment: "Докуента е валиден"),
                    CurrentPeriod = table.Column<int>(type: "int", nullable: false, comment: "Текущ период"),
                    Price = table.Column<float>(type: "real", nullable: false, comment: "Единична цена"),
                    Order = table.Column<int>(type: "int", nullable: false, comment: "Номер по ред"),
                    HasSerialNumber = table.Column<bool>(type: "bit", nullable: false, comment: "Има серийни номера"),
                    IsDestroyable = table.Column<bool>(type: "bit", nullable: false, comment: "IsDestroyable"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_TypeOfRequestedDocument", x => x.IdTypeOfRequestedDocument);
                });

            migrationBuilder.CreateTable(
                name: "Request_RequestDocumentType",
                columns: table => new
                {
                    IdRequestDocumentType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  CPO,CIPO - Обучаваща институция"),
                    IdProviderRequestDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка със заявка подадена от ЦПО/ЦИПО"),
                    IdTypeOfRequestedDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Тип документ"),
                    DocumentCount = table.Column<int>(type: "int", nullable: false, comment: "Брой документи"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_RequestDocumentType", x => x.IdRequestDocumentType);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentType_Provider_IdProvider",
                        column: x => x.IdProvider,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentType_Request_ProviderRequestDocument_IdProviderRequestDocument",
                        column: x => x.IdProviderRequestDocument,
                        principalTable: "Request_ProviderRequestDocument",
                        principalColumn: "IdProviderRequestDocument",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentType_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                        column: x => x.IdTypeOfRequestedDocument,
                        principalTable: "Request_TypeOfRequestedDocument",
                        principalColumn: "IdTypeOfRequestedDocument",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentType_IdProvider",
                table: "Request_RequestDocumentType",
                column: "IdProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentType_IdProviderRequestDocument",
                table: "Request_RequestDocumentType",
                column: "IdProviderRequestDocument");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentType_IdTypeOfRequestedDocument",
                table: "Request_RequestDocumentType",
                column: "IdTypeOfRequestedDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_RequestDocumentType");

            migrationBuilder.DropTable(
                name: "Request_TypeOfRequestedDocument");
        }
    }
}
