using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableForTraining : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_Client",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Име"),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Презиме"),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Фамилия"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    IdSex = table.Column<int>(type: "int", nullable: true, comment: "Пол"),
                    IdIndentType = table.Column<int>(type: "int", nullable: true, comment: "Вид на идентификатора"),
                    Indent = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "ЕГН/ЛНЧ/ИДН"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на раждане"),
                    IdNationality = table.Column<int>(type: "int", nullable: true, comment: "Гражданство"),
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: false),
                    IdEducation = table.Column<int>(type: "int", nullable: true, comment: "Образование"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Client", x => x.IdClient);
                    table.ForeignKey(
                        name: "FK_Training_Client_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_Client_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                        column: x => x.IdProfessionalDirection,
                        principalTable: "SPPOO_ProfessionalDirection",
                        principalColumn: "IdProfessionalDirection",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Получател на услугата(обучаем)");

            migrationBuilder.CreateTable(
                name: "Training_Program",
                columns: table => new
                {
                    IdProgram = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramNumber = table.Column<int>(type: "int", nullable: false, comment: "Номер на програма"),
                    ProgramName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Наименование на програма"),
                    ProgramNote = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "Допълнителна информация"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false, comment: "Специалност"),
                    IdFrameworkProgram = table.Column<int>(type: "int", nullable: false, comment: "Рамкова програма"),
                    IdFormEducation = table.Column<int>(type: "int", nullable: false, comment: "Форма на обучение"),
                    IdCourseType = table.Column<int>(type: "int", nullable: false, comment: "Вид на обучение"),
                    MandatoryHours = table.Column<int>(type: "int", nullable: false, comment: "Задължителни учебни ч.(бр.)"),
                    SelectableHours = table.Column<int>(type: "int", nullable: false, comment: "Избираеми учебни ч.(бр.)"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Program", x => x.IdProgram);
                    table.ForeignKey(
                        name: "FK_Training_Program_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_Program_SPPOO_FrameworkProgram_IdFrameworkProgram",
                        column: x => x.IdFrameworkProgram,
                        principalTable: "SPPOO_FrameworkProgram",
                        principalColumn: "IdFrameworkProgram",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_Program_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Програмa за обучение, предлагани от ЦПО");

            migrationBuilder.CreateTable(
                name: "Training_Course",
                columns: table => new
                {
                    IdCourse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProgram = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Програмa за обучение, предлагани от ЦПО"),
                    SubscribeDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Крайна дата за записване"),
                    CourseName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Наименование на курса"),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true, comment: "Други пояснения"),
                    IdStatus = table.Column<int>(type: "int", nullable: true, comment: "Статус на курса"),
                    IdMeasureType = table.Column<int>(type: "int", nullable: true, comment: "Вид"),
                    IdAssignType = table.Column<int>(type: "int", nullable: true, comment: "Основен източник на финансиране"),
                    IdFormEducation = table.Column<int>(type: "int", nullable: true, comment: "Форма на обучение"),
                    IdLocation = table.Column<int>(type: "int", nullable: true, comment: "Населено място"),
                    MandatoryHours = table.Column<int>(type: "int", nullable: true, comment: "Задължителни учебни ч.(бр.)"),
                    SelectableHours = table.Column<int>(type: "int", nullable: true, comment: "Избираеми учебни ч.(бр.)"),
                    DurationHours = table.Column<int>(type: "int", nullable: true, comment: "Продължителност"),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true, comment: "Цена (в лева за един курсист)"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Очаквана дата за започване на курса"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Очаквана дата за завършване на курса"),
                    ExamTheoryDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Очаквана дата за изпит по теория"),
                    ExamPracticeDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Очаквана дата за изпит по практика"),
                    ExamCommMmbers = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Членове на изпитната комисия (име и институция, която представлява)"),
                    IdCandidateProviderPremises = table.Column<int>(type: "int", nullable: false, comment: "Метериална техническа база"),
                    DisabilityCount = table.Column<int>(type: "int", nullable: true, comment: "Брой обучаеми в неравностойно положение (параграф 1 – т. 4а от ЗНЗ)"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_Course", x => x.IdCourse);
                    table.ForeignKey(
                        name: "FK_Training_Course_Candidate_ProviderPremises_IdCandidateProviderPremises",
                        column: x => x.IdCandidateProviderPremises,
                        principalTable: "Candidate_ProviderPremises",
                        principalColumn: "IdCandidateProviderPremises",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_Course_Location_IdLocation",
                        column: x => x.IdLocation,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                    table.ForeignKey(
                        name: "FK_Training_Course_Training_Program_IdProgram",
                        column: x => x.IdProgram,
                        principalTable: "Training_Program",
                        principalColumn: "IdProgram",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Курс за обучение, предлагани от ЦПО");

            migrationBuilder.CreateTable(
                name: "Training_ClientCourse",
                columns: table => new
                {
                    IdClientCourse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Получател на услугата(обучаем)"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за обучение, предлагани от ЦПО"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Име"),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Презиме"),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Фамилия"),
                    IdSex = table.Column<int>(type: "int", nullable: true, comment: "Пол"),
                    IdIndentType = table.Column<int>(type: "int", nullable: true, comment: "Вид на идентификатора"),
                    Indent = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "ЕГН/ЛНЧ/ИДН"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на раждане"),
                    IdNationality = table.Column<int>(type: "int", nullable: true, comment: "Гражданство"),
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: false),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdEducation = table.Column<int>(type: "int", nullable: true, comment: "Образование"),
                    IdAssignType = table.Column<int>(type: "int", nullable: true, comment: "Основен източник на финансиране"),
                    IdFinishedType = table.Column<int>(type: "int", nullable: true, comment: "Приключване на курс"),
                    FinishedDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на приключване на курса"),
                    IdQualificationLevel = table.Column<int>(type: "int", nullable: true, comment: "Придобита квалификация"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ClientCourse", x => x.IdClientCourse);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourse_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                        column: x => x.IdProfessionalDirection,
                        principalTable: "SPPOO_ProfessionalDirection",
                        principalColumn: "IdProfessionalDirection",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourse_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourse_Training_Client_IdClient",
                        column: x => x.IdClient,
                        principalTable: "Training_Client",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourse_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Получател на услугата(обучаем) връзка с курс");

            migrationBuilder.CreateTable(
                name: "Training_TrainerCourse",
                columns: table => new
                {
                    IdTrainerCourse = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTrainer = table.Column<int>(type: "int", nullable: false),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Курс за обучение, предлагани от ЦПО"),
                    IdТraininType = table.Column<int>(type: "int", nullable: true, comment: "Вид обучение"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_TrainerCourse", x => x.IdTrainerCourse);
                    table.ForeignKey(
                        name: "FK_Training_TrainerCourse_Candidate_ProviderTrainer_IdTrainer",
                        column: x => x.IdTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_TrainerCourse_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Връзка между лектор и курс");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Client_IdCandidateProvider",
                table: "Training_Client",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Client_IdProfessionalDirection",
                table: "Training_Client",
                column: "IdProfessionalDirection");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourse_IdClient",
                table: "Training_ClientCourse",
                column: "IdClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourse_IdCourse",
                table: "Training_ClientCourse",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourse_IdProfessionalDirection",
                table: "Training_ClientCourse",
                column: "IdProfessionalDirection");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourse_IdSpeciality",
                table: "Training_ClientCourse",
                column: "IdSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Course_IdCandidateProviderPremises",
                table: "Training_Course",
                column: "IdCandidateProviderPremises");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Course_IdLocation",
                table: "Training_Course",
                column: "IdLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Course_IdProgram",
                table: "Training_Course",
                column: "IdProgram");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Program_IdCandidateProvider",
                table: "Training_Program",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Program_IdFrameworkProgram",
                table: "Training_Program",
                column: "IdFrameworkProgram");

            migrationBuilder.CreateIndex(
                name: "IX_Training_Program_IdSpeciality",
                table: "Training_Program",
                column: "IdSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_Training_TrainerCourse_IdCourse",
                table: "Training_TrainerCourse",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_TrainerCourse_IdTrainer",
                table: "Training_TrainerCourse",
                column: "IdTrainer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ClientCourse");

            migrationBuilder.DropTable(
                name: "Training_TrainerCourse");

            migrationBuilder.DropTable(
                name: "Training_Client");

            migrationBuilder.DropTable(
                name: "Training_Course");

            migrationBuilder.DropTable(
                name: "Training_Program");
        }
    }
}
