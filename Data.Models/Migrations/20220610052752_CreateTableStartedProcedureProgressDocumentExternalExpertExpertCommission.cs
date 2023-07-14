using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableStartedProcedureProgressDocumentExternalExpertExpertCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procedure_StartedProcedure",
                columns: table => new
                {
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProvider = table.Column<int>(type: "int", nullable: false),
                    TS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NapooReportDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeetingHour = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LicenseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpertReportDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_StartedProcedure", x => x.IdStartedProcedure);
                    table.ForeignKey(
                        name: "FK_Procedure_StartedProcedure_Provider_IdProvider",
                        column: x => x.IdProvider,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procedure_Document",
                columns: table => new
                {
                    IdStartedProcedureProgress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: false),
                    IsValid = table.Column<bool>(type: "bit", nullable: false),
                    IdDocumentType = table.Column<int>(type: "int", nullable: true),
                    DateAttachment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    UIN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_ID = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_OFFICIAL_ID = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DS_OFFICIAL_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_PREP = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DS_LINK = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_Document", x => x.IdStartedProcedureProgress);
                    table.ForeignKey(
                        name: "FK_Procedure_Document_Procedure_StartedProcedure_IdStartedProcedure",
                        column: x => x.IdStartedProcedure,
                        principalTable: "Procedure_StartedProcedure",
                        principalColumn: "IdStartedProcedure",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procedure_ExpertCommission",
                columns: table => new
                {
                    IdProcedureExpertCommission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: false),
                    IdExpert = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_ExpertCommission", x => x.IdProcedureExpertCommission);
                    table.ForeignKey(
                        name: "FK_Procedure_ExpertCommission_ExpComm_Expert_IdExpert",
                        column: x => x.IdExpert,
                        principalTable: "ExpComm_Expert",
                        principalColumn: "IdExpert",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedure_ExpertCommission_Procedure_StartedProcedure_IdStartedProcedure",
                        column: x => x.IdStartedProcedure,
                        principalTable: "Procedure_StartedProcedure",
                        principalColumn: "IdStartedProcedure",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procedure_ExternalExpert",
                columns: table => new
                {
                    IdProcedureExternalExpert = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: false),
                    IdExpert = table.Column<int>(type: "int", nullable: false),
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_ExternalExpert", x => x.IdProcedureExternalExpert);
                    table.ForeignKey(
                        name: "FK_Procedure_ExternalExpert_ExpComm_Expert_IdExpert",
                        column: x => x.IdExpert,
                        principalTable: "ExpComm_Expert",
                        principalColumn: "IdExpert",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedure_ExternalExpert_Procedure_StartedProcedure_IdStartedProcedure",
                        column: x => x.IdStartedProcedure,
                        principalTable: "Procedure_StartedProcedure",
                        principalColumn: "IdStartedProcedure",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedure_ExternalExpert_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                        column: x => x.IdProfessionalDirection,
                        principalTable: "SPPOO_ProfessionalDirection",
                        principalColumn: "IdProfessionalDirection");
                });

            migrationBuilder.CreateTable(
                name: "Procedure_StartedProcedureProgress",
                columns: table => new
                {
                    IdStartedProcedureProgress = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: false),
                    IdStep = table.Column<int>(type: "int", nullable: true),
                    StepDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_StartedProcedureProgress", x => x.IdStartedProcedureProgress);
                    table.ForeignKey(
                        name: "FK_Procedure_StartedProcedureProgress_Procedure_StartedProcedure_IdStartedProcedure",
                        column: x => x.IdStartedProcedure,
                        principalTable: "Procedure_StartedProcedure",
                        principalColumn: "IdStartedProcedure",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_Document_IdStartedProcedure",
                table: "Procedure_Document",
                column: "IdStartedProcedure");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ExpertCommission_IdExpert",
                table: "Procedure_ExpertCommission",
                column: "IdExpert");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ExpertCommission_IdStartedProcedure",
                table: "Procedure_ExpertCommission",
                column: "IdStartedProcedure");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ExternalExpert_IdExpert",
                table: "Procedure_ExternalExpert",
                column: "IdExpert");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ExternalExpert_IdProfessionalDirection",
                table: "Procedure_ExternalExpert",
                column: "IdProfessionalDirection");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_ExternalExpert_IdStartedProcedure",
                table: "Procedure_ExternalExpert",
                column: "IdStartedProcedure");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_StartedProcedure_IdProvider",
                table: "Procedure_StartedProcedure",
                column: "IdProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_StartedProcedureProgress_IdStartedProcedure",
                table: "Procedure_StartedProcedureProgress",
                column: "IdStartedProcedure");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedure_Document");

            migrationBuilder.DropTable(
                name: "Procedure_ExpertCommission");

            migrationBuilder.DropTable(
                name: "Procedure_ExternalExpert");

            migrationBuilder.DropTable(
                name: "Procedure_StartedProcedureProgress");

            migrationBuilder.DropTable(
                name: "Procedure_StartedProcedure");
        }
    }
}
