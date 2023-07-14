using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableArchCandidateProvider_ArchCandidateProviderTrainer_ArchCandidateProviderTrainerQualification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_Candidate_Provider",
                columns: table => new
                {
                    IdArchCandidateProvider = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false, comment: "Година"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    ProviderOwner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProviderOwnerEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Наименование на юридическото лице на латиница"),
                    PoviderBulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ManagerName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AttorneyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Indent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IdProviderRegistration = table.Column<int>(type: "int", nullable: false),
                    IdProviderOwnership = table.Column<int>(type: "int", nullable: false),
                    IdProviderStatus = table.Column<int>(type: "int", nullable: false),
                    IdLocation = table.Column<int>(type: "int", nullable: true),
                    ProviderAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IdRegionAdmin = table.Column<int>(type: "int", nullable: true),
                    ProviderName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    ProviderNameEN = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Име на ЦПО,ЦИПО на Латиница"),
                    IdTypeLicense = table.Column<int>(type: "int", nullable: false),
                    ApplicationNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ApplicationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Номер на лиценза"),
                    LicenceDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на получаване на лицензия"),
                    IdLicenceStatus = table.Column<int>(type: "int", nullable: true, comment: "Статус на  лицензията"),
                    ProviderPhone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderFax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderWeb = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    AccessibilityInfo = table.Column<bool>(type: "bit", nullable: false),
                    OnlineTrainingInfo = table.Column<bool>(type: "bit", nullable: false),
                    IdTypeApplication = table.Column<int>(type: "int", nullable: true),
                    IdApplicationStatus = table.Column<int>(type: "int", nullable: true, comment: "Статус на  заявлението"),
                    IdRegistrationApplicationStatus = table.Column<int>(type: "int", nullable: true, comment: "Статус на регистрация на заявлението"),
                    RejectionReason = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Причина за отказ"),
                    DateRequest = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDateRequest = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateConfirmRequestNAPOO = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateConfirmEMail = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdReceiveLicense = table.Column<int>(type: "int", nullable: true, comment: "Начин на получаване на административен акт и лицензия"),
                    IdApplicationFiling = table.Column<int>(type: "int", nullable: true, comment: "Начин на подаване на заявление и документ за платена държавна такса"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Определя активния запис за CandidateProvider"),
                    UIN = table.Column<long>(type: "bigint", nullable: true, comment: "Уникален идентификатор за връзка с деловодната система на НАПОО"),
                    DirectorFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Име на директор на ЦПО,ЦИПО"),
                    DirectorSecondName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Презиме на директор на ЦПО,ЦИПО"),
                    DirectorFamilyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Фамилия директор на ЦПО,ЦИПО"),
                    PersonNameCorrespondence = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PersonNameCorrespondenceEN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Лице за контакт/кореспондениця на латиница"),
                    IdLocationCorrespondence = table.Column<int>(type: "int", nullable: true),
                    IdRegionCorrespondence = table.Column<int>(type: "int", nullable: true),
                    ProviderAddressCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderAddressCorrespondenceEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Адрес за кореспонденция   на ЦПО,ЦИПО на латиница"),
                    ZipCodeCorrespondence = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    ProviderPhoneCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderFaxCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProviderEmailCorrespondence = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: true),
                    IdCandidateProviderActive = table.Column<int>(type: "int", nullable: true, comment: "Връзка с активния канидат провайдър"),
                    ESignApplicationFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Път до електронно подписанато заявление"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_Candidate_Provider", x => x.IdArchCandidateProvider);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Provider_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Provider_Candidate_Provider_IdCandidateProviderActive",
                        column: x => x.IdCandidateProviderActive,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider");
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Provider_Location_IdLocation",
                        column: x => x.IdLocation,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Provider_Location_IdLocationCorrespondence",
                        column: x => x.IdLocationCorrespondence,
                        principalTable: "Location",
                        principalColumn: "idLocation");
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Provider_Procedure_StartedProcedure_IdStartedProcedure",
                        column: x => x.IdStartedProcedure,
                        principalTable: "Procedure_StartedProcedure",
                        principalColumn: "IdStartedProcedure");
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Provider_Region_IdRegionAdmin",
                        column: x => x.IdRegionAdmin,
                        principalTable: "Region",
                        principalColumn: "idRegion");
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_Provider_Region_IdRegionCorrespondence",
                        column: x => x.IdRegionCorrespondence,
                        principalTable: "Region",
                        principalColumn: "idRegion");
                });

            migrationBuilder.CreateTable(
                name: "Arch_Candidate_ProviderTrainer",
                columns: table => new
                {
                    IdArchCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false),
                    IdArchCandidateProvider = table.Column<int>(type: "int", nullable: false),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecondName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FamilyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdIndentType = table.Column<int>(type: "int", nullable: true),
                    Indent = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdSex = table.Column<int>(type: "int", nullable: true),
                    IdNationality = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdEducation = table.Column<int>(type: "int", nullable: false),
                    ProfessionalQualificationCertificate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EducationSpecialityNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EducationCertificateNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EducationAcademicNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsAndragog = table.Column<bool>(type: "bit", nullable: false),
                    IdContractType = table.Column<int>(type: "int", nullable: true),
                    ContractDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdStatus = table.Column<int>(type: "int", nullable: true),
                    InactiveDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на деактивиране на преподавателя/консултанта"),
                    DiplomaNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Номер на диплома"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_Candidate_ProviderTrainer", x => x.IdArchCandidateProviderTrainer);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderTrainer_Arch_Candidate_Provider_IdArchCandidateProvider",
                        column: x => x.IdArchCandidateProvider,
                        principalTable: "Arch_Candidate_Provider",
                        principalColumn: "IdArchCandidateProvider",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderTrainer_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Arch_Candidate_ProviderTrainerQualification",
                columns: table => new
                {
                    IdArchCandidateProviderTrainerQualification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProviderTrainerQualification = table.Column<int>(type: "int", nullable: false),
                    IdArchCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false, comment: "АРХИВ Връзка с Преподавател"),
                    IdCandidateProviderTrainer = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Преподавател"),
                    QualificationName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IdQualificationType = table.Column<int>(type: "int", nullable: false),
                    IdProfession = table.Column<int>(type: "int", nullable: true),
                    IdTrainingQualificationType = table.Column<int>(type: "int", nullable: false),
                    QualificationDuration = table.Column<int>(type: "int", nullable: true),
                    TrainingFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrainingTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_Candidate_ProviderTrainerQualification", x => x.IdArchCandidateProviderTrainerQualification);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderTrainerQualification_Arch_Candidate_ProviderTrainer_IdArchCandidateProviderTrainer",
                        column: x => x.IdArchCandidateProviderTrainer,
                        principalTable: "Arch_Candidate_ProviderTrainer",
                        principalColumn: "IdArchCandidateProviderTrainer",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderTrainerQualification_Candidate_ProviderTrainer_IdCandidateProviderTrainer",
                        column: x => x.IdCandidateProviderTrainer,
                        principalTable: "Candidate_ProviderTrainer",
                        principalColumn: "IdCandidateProviderTrainer",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Arch_Candidate_ProviderTrainerQualification_SPPOO_Profession_IdProfession",
                        column: x => x.IdProfession,
                        principalTable: "SPPOO_Profession",
                        principalColumn: "IdProfession");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdCandidate_Provider",
                table: "Arch_Candidate_Provider",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdCandidateProviderActive",
                table: "Arch_Candidate_Provider",
                column: "IdCandidateProviderActive");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdLocation",
                table: "Arch_Candidate_Provider",
                column: "IdLocation");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdLocationCorrespondence",
                table: "Arch_Candidate_Provider",
                column: "IdLocationCorrespondence");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdRegionAdmin",
                table: "Arch_Candidate_Provider",
                column: "IdRegionAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdRegionCorrespondence",
                table: "Arch_Candidate_Provider",
                column: "IdRegionCorrespondence");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_Provider_IdStartedProcedure",
                table: "Arch_Candidate_Provider",
                column: "IdStartedProcedure");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderTrainer_IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderTrainer",
                column: "IdArchCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderTrainer_IdCandidate_Provider",
                table: "Arch_Candidate_ProviderTrainer",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderTrainerQualification_IdArchCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                column: "IdArchCandidateProviderTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderTrainerQualification_IdCandidateProviderTrainer",
                table: "Arch_Candidate_ProviderTrainerQualification",
                column: "IdCandidateProviderTrainer");

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderTrainerQualification_IdProfession",
                table: "Arch_Candidate_ProviderTrainerQualification",
                column: "IdProfession");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_Candidate_ProviderTrainerQualification");

            migrationBuilder.DropTable(
                name: "Arch_Candidate_ProviderTrainer");

            migrationBuilder.DropTable(
                name: "Arch_Candidate_Provider");
        }
    }
}
