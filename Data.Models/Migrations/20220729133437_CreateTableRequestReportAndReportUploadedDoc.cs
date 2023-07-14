using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableRequestReportAndReportUploadedDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRequestReport",
                table: "Request_RequestDocumentManagement",
                type: "int",
                nullable: true,
                comment: "Отчет на документи с фабрична номерация по наредба 8");

            migrationBuilder.CreateTable(
                name: "Request_Report",
                columns: table => new
                {
                    IdRequestReport = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус на одобрение на отчета"),
                    DestructionDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на унищожаване"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_Report", x => x.IdRequestReport);
                    table.ForeignKey(
                        name: "FK_Request_Report_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Request_ReportUploadedDoc",
                columns: table => new
                {
                    IdReportUploadedDoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    IdRequestReport = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    IdTypeReportUploadedDocument = table.Column<int>(type: "int", nullable: false, comment: "Тип документ"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Описание"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_ReportUploadedDoc", x => x.IdReportUploadedDoc);
                    table.ForeignKey(
                        name: "FK_Request_ReportUploadedDoc_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Request_ReportUploadedDoc_Request_Report_IdRequestReport",
                        column: x => x.IdRequestReport,
                        principalTable: "Request_Report",
                        principalColumn: "IdRequestReport",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentManagement_IdRequestReport",
                table: "Request_RequestDocumentManagement",
                column: "IdRequestReport");

            migrationBuilder.CreateIndex(
                name: "IX_Request_Report_IdCandidateProvider",
                table: "Request_Report",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ReportUploadedDoc_IdCandidateProvider",
                table: "Request_ReportUploadedDoc",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Request_ReportUploadedDoc_IdRequestReport",
                table: "Request_ReportUploadedDoc",
                column: "IdRequestReport");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentManagement_Request_Report_IdRequestReport",
                table: "Request_RequestDocumentManagement",
                column: "IdRequestReport",
                principalTable: "Request_Report",
                principalColumn: "IdRequestReport");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentManagement_Request_Report_IdRequestReport",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropTable(
                name: "Request_ReportUploadedDoc");

            migrationBuilder.DropTable(
                name: "Request_Report");

            migrationBuilder.DropIndex(
                name: "IX_Request_RequestDocumentManagement_IdRequestReport",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropColumn(
                name: "IdRequestReport",
                table: "Request_RequestDocumentManagement");
        }
    }
}
