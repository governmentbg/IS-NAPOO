using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_ValidationClient_ValidationClientDocument_ValidationProtocol_ValidationCommissionMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ValidationClient",
                columns: table => new
                {
                    IdValidationClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Получател на услугата(обучаем)"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CandidateProvider"),
                    IdSpeciality = table.Column<int>(type: "int", nullable: true),
                    IdFrameworkProgram = table.Column<int>(type: "int", nullable: true, comment: "Рамкова програма"),
                    IdQualificationLevel = table.Column<int>(type: "int", nullable: true, comment: "Придобита квалификация"),
                    IdCourseType = table.Column<int>(type: "int", nullable: false, comment: "Вид на курса за обучение"),
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
                    ExamTheoryDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Очаквана дата за изпит по теория"),
                    ExamPracticeDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Очаквана дата за изпит по практика"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationClient", x => x.IdValidationClient);
                    table.ForeignKey(
                        name: "FK_Training_ValidationClient_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ValidationClient_SPPOO_FrameworkProgram_IdFrameworkProgram",
                        column: x => x.IdFrameworkProgram,
                        principalTable: "SPPOO_FrameworkProgram",
                        principalColumn: "IdFrameworkProgram");
                    table.ForeignKey(
                        name: "FK_Training_ValidationClient_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality");
                    table.ForeignKey(
                        name: "FK_Training_ValidationClient_Training_Client_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Training_Client",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Получател на услугата по валидиране на специалност");

            migrationBuilder.CreateTable(
                name: "Training_ValidationCommissionMember",
                columns: table => new
                {
                    IdValidationCommissionMember = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false, comment: "Име"),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Презиме"),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Фамилия"),
                    IsChairman = table.Column<bool>(type: "bit", nullable: false, comment: "Дали е председател на комисия"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с клиента по процедура за валидиране"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationCommissionMember", x => x.IdValidationCommissionMember);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCommissionMember_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Членове на изпитна комисия към процедура за валидиране");

            migrationBuilder.CreateTable(
                name: "Training_ValidationProtocol",
                columns: table => new
                {
                    IdValidationProtocol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с клиент по процедура за валидиране"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с ЦПО"),
                    ValidationProtocolNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Номер на протокол"),
                    ValidationProtocolDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на протокол"),
                    IdValidationProtocolType = table.Column<int>(type: "int", nullable: false, comment: "Вид на протокол"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationProtocol", x => x.IdValidationProtocol);
                    table.ForeignKey(
                        name: "FK_Training_ValidationProtocol_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ValidationProtocol_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Протокол към процедура за валидиране");

            migrationBuilder.CreateTable(
                name: "Training_ValidationCourseDocument",
                columns: table => new
                {
                    IdValidationClientDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с обучаем"),
                    IdValidationProtocol = table.Column<int>(type: "int", nullable: true, comment: "Връзка с протокол от курс за валидиране"),
                    IdDocumentSerialNumber = table.Column<int>(type: "int", nullable: true, comment: "Връзка с фабричен номер на документ от печатница на МОН"),
                    IdDocumentType = table.Column<int>(type: "int", nullable: true, comment: "Документи за завършено обучение"),
                    FinishedYear = table.Column<int>(type: "int", nullable: true, comment: "Година на приключване"),
                    DocumentRegNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Регистрационен номер"),
                    DocumentDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на регистрационен документ"),
                    DocumentProtocol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Протокол"),
                    TheoryResult = table.Column<decimal>(type: "decimal(3,2)", nullable: true, comment: "Оценка по теория"),
                    PracticeResult = table.Column<decimal>(type: "decimal(3,2)", nullable: true, comment: "Оценка по практика"),
                    FinalResult = table.Column<decimal>(type: "decimal(3,2)", nullable: true, comment: "Обща оценка от теория и практика"),
                    QualificationLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Степен"),
                    IdDocumentStatus = table.Column<int>(type: "int", nullable: true, comment: "Статус на документ за завършено обучение"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationCourseDocument", x => x.IdValidationClientDocument);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCourseDocument_Request_DocumentSerialNumber_IdDocumentSerialNumber",
                        column: x => x.IdDocumentSerialNumber,
                        principalTable: "Request_DocumentSerialNumber",
                        principalColumn: "IdDocumentSerialNumber");
                    table.ForeignKey(
                        name: "FK_Training_ValidationCourseDocument_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCourseDocument_Training_ValidationProtocol_IdValidationProtocol",
                        column: x => x.IdValidationProtocol,
                        principalTable: "Training_ValidationProtocol",
                        principalColumn: "IdValidationProtocol");
                },
                comment: "Издадени документи на клиенти по процедура за валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClient_IdCandidateProvider",
                table: "Training_ValidationClient",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClient_IdClient",
                table: "Training_ValidationClient",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClient_IdFrameworkProgram",
                table: "Training_ValidationClient",
                column: "IdFrameworkProgram");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClient_IdSpeciality",
                table: "Training_ValidationClient",
                column: "IdSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCommissionMember_IdValidationClient",
                table: "Training_ValidationCommissionMember",
                column: "IdValidationClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCourseDocument_IdDocumentSerialNumber",
                table: "Training_ValidationCourseDocument",
                column: "IdDocumentSerialNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCourseDocument_IdValidationClient",
                table: "Training_ValidationCourseDocument",
                column: "IdValidationClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCourseDocument_IdValidationProtocol",
                table: "Training_ValidationCourseDocument",
                column: "IdValidationProtocol");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationProtocol_IdCandidateProvider",
                table: "Training_ValidationProtocol",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationProtocol_IdValidationClient",
                table: "Training_ValidationProtocol",
                column: "IdValidationClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationCommissionMember");

            migrationBuilder.DropTable(
                name: "Training_ValidationCourseDocument");

            migrationBuilder.DropTable(
                name: "Training_ValidationProtocol");

            migrationBuilder.DropTable(
                name: "Training_ValidationClient");
        }
    }
}
