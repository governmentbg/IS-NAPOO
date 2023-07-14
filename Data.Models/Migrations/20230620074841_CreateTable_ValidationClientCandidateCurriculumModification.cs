using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_ValidationClientCandidateCurriculumModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedCurriculumFileName",
                table: "Training_ValidationClient",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Прикачен файл с учебна програма");

            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Training_Course",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Прикачен файл с учебна програма");

            migrationBuilder.CreateTable(
                name: "Training_ValidationClientCandidateCurriculumModification",
                columns: table => new
                {
                    IdValidationClientCandidateCurriculumModification = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с валидирано лице"),
                    IdCandidateCurriculumModification = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Програмa за обучение, предлагани от ЦПО"),
                    OldId = table.Column<int>(type: "int", nullable: true),
                    MigrationNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationClientCandidateCurriculumModification", x => x.IdValidationClientCandidateCurriculumModification);
                    table.ForeignKey(
                        name: "FK_Training_ValidationClientCandidateCurriculumModification_Candidate_CurriculumModification_IdCandidateCurriculumModification",
                        column: x => x.IdCandidateCurriculumModification,
                        principalTable: "Candidate_CurriculumModification",
                        principalColumn: "IdCandidateCurriculumModification",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ValidationClientCandidateCurriculumModification_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Връзка между валидирано лице и промяна на учебен план");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientCandidateCurriculumModification_IdCandidateCurriculumModification",
                table: "Training_ValidationClientCandidateCurriculumModification",
                column: "IdCandidateCurriculumModification");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationClientCandidateCurriculumModification_IdValidationClient",
                table: "Training_ValidationClientCandidateCurriculumModification",
                column: "IdValidationClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationClientCandidateCurriculumModification");

            migrationBuilder.DropColumn(
                name: "UploadedCurriculumFileName",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Training_Course");
        }
    }
}
