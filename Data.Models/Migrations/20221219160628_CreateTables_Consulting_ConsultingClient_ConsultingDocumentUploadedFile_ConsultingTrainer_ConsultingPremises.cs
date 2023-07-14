using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_Consulting_ConsultingClient_ConsultingDocumentUploadedFile_ConsultingTrainer_ConsultingPremises : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ConsultingClient",
                columns: table => new
                {
                    IdConsultingClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Получател на услугата(обучаем)"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CandidateProvider"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Име"),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Презиме"),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Фамилия"),
                    IdSex = table.Column<int>(type: "int", nullable: true, comment: "Пол"),
                    IdIndentType = table.Column<int>(type: "int", nullable: true, comment: "Вид на идентификатора"),
                    Indent = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "ЕГН/ЛНЧ/ИДН"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на раждане"),
                    IdNationality = table.Column<int>(type: "int", nullable: true, comment: "Гражданство"),
                    IdAssignType = table.Column<int>(type: "int", nullable: true, comment: "Основен източник на финансиране"),
                    IdFinishedType = table.Column<int>(type: "int", nullable: true, comment: "Приключване на курс"),
                    IdCountryOfBirth = table.Column<int>(type: "int", nullable: true, comment: "Месторождение (държава)"),
                    IdCityOfBirth = table.Column<int>(type: "int", nullable: true, comment: "Месторождение (населено място)"),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true, comment: "Цена (в лева за консултирано лице)"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на стартиране на консултацията"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на приключване на консултацията"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ConsultingClient", x => x.IdConsultingClient);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingClient_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingClient_Training_Client_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Training_Client",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Получател на услугата(консултиран) по дейност от ЦИПО");

            migrationBuilder.CreateTable(
                name: "Training_Consulting",
                columns: table => new
                {
                    IdConsulting = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConsultingClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с консултирано лице"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Consulting", x => x.IdConsulting);
                    table.ForeignKey(
                        name: "FK_Training_Consulting_Training_ConsultingClient_IdConsultingClient",
                        column: x => x.IdConsultingClient,
                        principalTable: "Training_ConsultingClient",
                        principalColumn: "IdConsultingClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Консултация по дейности, предлагани от ЦИПО");

            migrationBuilder.CreateTable(
                name: "Training_ConsultingDocumentUploadedFile",
                columns: table => new
                {
                    IdConsultingDocumentUploadedFile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConsultingClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с консултирано лице"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ConsultingDocumentUploadedFile", x => x.IdConsultingDocumentUploadedFile);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingDocumentUploadedFile_Training_ConsultingClient_IdConsultingClient",
                        column: x => x.IdConsultingClient,
                        principalTable: "Training_ConsultingClient",
                        principalColumn: "IdConsultingClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Прикачени файлове за документи на консултирано лице");

            migrationBuilder.CreateTable(
                name: "Training_ConsultingPremises",
                columns: table => new
                {
                    IdConsultingPremises = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPremises = table.Column<int>(type: "int", nullable: false),
                    IdConsultingClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с консултирано лице"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ConsultingPremises", x => x.IdConsultingPremises);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingPremises_Candidate_ProviderPremises_IdPremises",
                        column: x => x.IdPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingPremises_Training_ConsultingClient_IdConsultingClient",
                        column: x => x.IdConsultingClient,
                        principalTable: "Training_ConsultingClient",
                        principalColumn: "IdConsultingClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Връзка между MTB и консултация по дейност");

            migrationBuilder.CreateTable(
                name: "Training_ConsultingTrainer",
                columns: table => new
                {
                    IdConsultingTrainer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTrainer = table.Column<int>(type: "int", nullable: false),
                    IdConsultingClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с консултирано лице"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ConsultingTrainer", x => x.IdConsultingTrainer);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingTrainer_Candidate_ProviderTrainer_IdTrainer",
                        column: x => x.IdTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingTrainer_Training_ConsultingClient_IdConsultingClient",
                        column: x => x.IdConsultingClient,
                        principalTable: "Training_ConsultingClient",
                        principalColumn: "IdConsultingClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Връзка между консултант и дейност по консултиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Consulting_IdConsultingClient",
                table: "Training_Consulting",
                column: "IdConsultingClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingClient_IdCandidateProvider",
                table: "Training_ConsultingClient",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingClient_IdClient",
                table: "Training_ConsultingClient",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingDocumentUploadedFile_IdConsultingClient",
                table: "Training_ConsultingDocumentUploadedFile",
                column: "IdConsultingClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingPremises_IdConsultingClient",
                table: "Training_ConsultingPremises",
                column: "IdConsultingClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingPremises_IdPremises",
                table: "Training_ConsultingPremises",
                column: "IdPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingTrainer_IdConsultingClient",
                table: "Training_ConsultingTrainer",
                column: "IdConsultingClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingTrainer_IdTrainer",
                table: "Training_ConsultingTrainer",
                column: "IdTrainer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ConsultingDocumentUploadedFile");

            migrationBuilder.DropTable(
                name: "Training_ConsultingPremises");

            migrationBuilder.DropTable(
                name: "Training_ConsultingTrainer");

            migrationBuilder.DropTable(
                name: "Training_ConsultingClient");

            migrationBuilder.DropTable(
                name: "Training_Consulting");
        }
    }
}
