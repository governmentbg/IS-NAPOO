using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_RequestDocumentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_RequestDocumentStatus",
                columns: table => new
                {
                    IdRequestDocumentStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  CPO,CIPO - Обучаваща институция"),
                    IdProviderRequestDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка със заявка подадена от ЦПО/ЦИПО"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_RequestDocumentStatus", x => x.IdRequestDocumentStatus);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentStatus_Provider_IdProvider",
                        column: x => x.IdProvider,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentStatus_Request_ProviderRequestDocument_IdProviderRequestDocument",
                        column: x => x.IdProviderRequestDocument,
                        principalTable: "Request_ProviderRequestDocument",
                        principalColumn: "IdProviderRequestDocument",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentStatus_IdProvider",
                table: "Request_RequestDocumentStatus",
                column: "IdProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentStatus_IdProviderRequestDocument",
                table: "Request_RequestDocumentStatus",
                column: "IdProviderRequestDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_RequestDocumentStatus");
        }
    }
}
