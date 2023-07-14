using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableRequest_ProviderDocumentOffer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_ProviderDocumentOffer",
                columns: table => new
                {
                    IdProviderDocumentOffer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  CPO - Обучаваща институция"),
                    IdOfferType = table.Column<int>(type: "int", nullable: false, comment: "Вид на оферта"),
                    IdTypeOfRequestedDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Тип документ"),
                    CountOffered = table.Column<int>(type: "int", nullable: false, comment: "Брой предлагани/търсени документи"),
                    OfferStartDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Начална дата на офертата"),
                    OfferEndDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Крайна дата на офертата"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_ProviderDocumentOffer", x => x.IdProviderDocumentOffer);
                    table.ForeignKey(
                        name: "FK_Request_ProviderDocumentOffer_Provider_IdProvider",
                        column: x => x.IdProvider,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Request_ProviderDocumentOffer_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                        column: x => x.IdTypeOfRequestedDocument,
                        principalTable: "Request_TypeOfRequestedDocument",
                        principalColumn: "IdTypeOfRequestedDocument",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProviderDocumentOffer_IdProvider",
                table: "Request_ProviderDocumentOffer",
                column: "IdProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProviderDocumentOffer_IdTypeOfRequestedDocument",
                table: "Request_ProviderDocumentOffer",
                column: "IdTypeOfRequestedDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_ProviderDocumentOffer");
        }
    }
}
