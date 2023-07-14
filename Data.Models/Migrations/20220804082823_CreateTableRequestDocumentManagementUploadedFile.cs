using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableRequestDocumentManagementUploadedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_RequestDocumentManagementUploadedFile",
                columns: table => new
                {
                    IdRequestDocumentManagementUploadedFile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRequestDocumentManagement = table.Column<int>(type: "int", nullable: false, comment: "Връзка със получени/предадени документи"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Описание"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_RequestDocumentManagementUploadedFile", x => x.IdRequestDocumentManagementUploadedFile);
                    table.ForeignKey(
                        name: "FK_Request_RequestDocumentManagementUploadedFile_Request_RequestDocumentManagement_IdRequestDocumentManagement",
                        column: x => x.IdRequestDocumentManagement,
                        principalTable: "Request_RequestDocumentManagement",
                        principalColumn: "IdRequestDocumentManagement",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentManagementUploadedFile_IdRequestDocumentManagement",
                table: "Request_RequestDocumentManagementUploadedFile",
                column: "IdRequestDocumentManagement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_RequestDocumentManagementUploadedFile");
        }
    }
}
