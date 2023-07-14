using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableProviderRequestDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_ProviderRequestDocument",
                columns: table => new
                {
                    IdProviderRequestDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProvider = table.Column<int>(type: "int", nullable: false),
                    IdNAPOORequestDoc = table.Column<int>(type: "int", nullable: true),
                    CurrentYear = table.Column<int>(type: "int", nullable: true, comment: "Година на заявка"),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на заявка"),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Длъжност на заявител"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Имена на заявител"),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Адрес"),
                    Telephone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Телефон"),
                    IsSent = table.Column<bool>(type: "bit", nullable: false, comment: "Заявката е изпратена към печатницата"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_ProviderRequestDocument", x => x.IdProviderRequestDocument);
                    table.ForeignKey(
                        name: "FK_Request_ProviderRequestDocument_Provider_IdProvider",
                        column: x => x.IdProvider,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Request_ProviderRequestDocument_Request_NAPOORequestDoc_IdNAPOORequestDoc",
                        column: x => x.IdNAPOORequestDoc,
                        principalTable: "Request_NAPOORequestDoc",
                        principalColumn: "IdNAPOORequestDoc");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProviderRequestDocument_IdNAPOORequestDoc",
                table: "Request_ProviderRequestDocument",
                column: "IdNAPOORequestDoc");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ProviderRequestDocument_IdProvider",
                table: "Request_ProviderRequestDocument",
                column: "IdProvider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_ProviderRequestDocument");
        }
    }
}
