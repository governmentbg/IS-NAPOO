using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableRequestDocumentManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_RequestDocumentManagement",
                columns: table => new
                {
                    IdRequestDocumentManagement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProviderRequestDocument = table.Column<int>(type: "int", nullable: true, comment: "Връзка със заявка за документация, подадена от ЦПО"),
                    IdProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  CPO - Обучаваща институция"),
                    IdProviderPartner = table.Column<int>(type: "int", nullable: true, comment: "Връзка с  CPO - Обучаваща институция"),
                    IdTypeOfRequestedDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Тип документ към печатница на МОН"),
                    DocumentCount = table.Column<int>(type: "int", nullable: false, comment: "Брой документи - Получени/Предадени"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на Получаване/Предаване"),
                    IdDocumentOperation = table.Column<int>(type: "int", nullable: false, comment: "Вид операция"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_RequestDocumentManagement", x => x.IdRequestDocumentManagement);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentManagement_Provider_IdProvider",
                        column: x => x.IdProvider,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentManagement_Provider_IdProviderPartner",
                        column: x => x.IdProviderPartner,
                        principalTable: "Provider",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentManagement_Request_ProviderRequestDocument_IdProviderRequestDocument",
                        column: x => x.IdProviderRequestDocument,
                        principalTable: "Request_ProviderRequestDocument",
                        principalColumn: "IdProviderRequestDocument");
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentManagement_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                        column: x => x.IdTypeOfRequestedDocument,
                        principalTable: "Request_TypeOfRequestedDocument",
                        principalColumn: "IdTypeOfRequestedDocument",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentManagement_IdProvider",
                table: "Request_RequestDocumentManagement",
                column: "IdProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentManagement_IdProviderPartner",
                table: "Request_RequestDocumentManagement",
                column: "IdProviderPartner");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentManagement_IdProviderRequestDocument",
                table: "Request_RequestDocumentManagement",
                column: "IdProviderRequestDocument");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentManagement_IdTypeOfRequestedDocument",
                table: "Request_RequestDocumentManagement",
                column: "IdTypeOfRequestedDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_RequestDocumentManagement");
        }
    }
}
