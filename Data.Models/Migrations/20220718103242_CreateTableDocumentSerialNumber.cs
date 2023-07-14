using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableDocumentSerialNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_DocumentSerialNumber",
                columns: table => new
                {
                    IdDocumentSerialNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRequestDocumentManagement = table.Column<int>(type: "int", nullable: false, comment: "Връзка със получени/предадени документи"),
                    IdProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  CPO - Обучаваща институция"),
                    IdTypeOfRequestedDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Тип документ към печатница на МОН"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на Получаване/Предаване"),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Сериен номер на документ"),
                    IdDocumentOperation = table.Column<int>(type: "int", nullable: false, comment: "Вид операция"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_DocumentSerialNumber", x => x.IdDocumentSerialNumber);
                    table.ForeignKey(
                        name: "FK_Request_DocumentSerialNumber_Provider_IdProvider",
                        column: x => x.IdProvider,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Request_DocumentSerialNumber_Request_RequestDocumentManagement_IdRequestDocumentManagement",
                        column: x => x.IdRequestDocumentManagement,
                        principalTable: "Request_RequestDocumentManagement",
                        principalColumn: "IdRequestDocumentManagement",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Request_DocumentSerialNumber_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                        column: x => x.IdTypeOfRequestedDocument,
                        principalTable: "Request_TypeOfRequestedDocument",
                        principalColumn: "IdTypeOfRequestedDocument",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_DocumentSerialNumber_IdProvider",
                table: "Request_DocumentSerialNumber",
                column: "IdProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Request_DocumentSerialNumber_IdRequestDocumentManagement",
                table: "Request_DocumentSerialNumber",
                column: "IdRequestDocumentManagement");

            migrationBuilder.CreateIndex(
                name: "IX_Request_DocumentSerialNumber_IdTypeOfRequestedDocument",
                table: "Request_DocumentSerialNumber",
                column: "IdTypeOfRequestedDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_DocumentSerialNumber");
        }
    }
}
