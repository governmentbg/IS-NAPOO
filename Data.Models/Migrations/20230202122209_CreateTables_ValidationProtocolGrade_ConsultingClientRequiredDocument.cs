using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_ValidationProtocolGrade_ConsultingClientRequiredDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ConsultingClientRequiredDocument",
                columns: table => new
                {
                    IdConsultingClientRequiredDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConsultingClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с консултирано лице"),
                    IdConsultingRequiredDocumentType = table.Column<int>(type: "int", nullable: false, comment: "Тип задължителни документи за курс,курсист"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Описание"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ConsultingClientRequiredDocument", x => x.IdConsultingClientRequiredDocument);
                    table.ForeignKey(
                        name: "FK_Training_ConsultingClientRequiredDocument_Training_ConsultingClient_IdConsultingClient",
                        column: x => x.IdConsultingClient,
                        principalTable: "Training_ConsultingClient",
                        principalColumn: "IdConsultingClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Издадени документи на консултирано лице");

            migrationBuilder.CreateTable(
                name: "Training_ValidationProtocolGrade",
                columns: table => new
                {
                    IdValidationProtocolGrade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationProtocol = table.Column<int>(type: "int", nullable: false, comment: "Връзка с протокол към курс за валидиране"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс за валидиране"),
                    Grade = table.Column<double>(type: "float", nullable: true, comment: "Оценка от протокол от курс за валидиране"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationProtocolGrade", x => x.IdValidationProtocolGrade);
                    table.ForeignKey(
                        name: "FK_Training_ValidationProtocolGrade_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Training_ValidationProtocolGrade_Training_ValidationProtocol_IdValidationProtocol",
                        column: x => x.IdValidationProtocol,
                        principalTable: "Training_ValidationProtocol",
                        principalColumn: "IdValidationProtocol",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Протокол към курс за валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ConsultingClientRequiredDocument_IdConsultingClient",
                table: "Training_ConsultingClientRequiredDocument",
                column: "IdConsultingClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationProtocolGrade_IdValidationClient",
                table: "Training_ValidationProtocolGrade",
                column: "IdValidationClient");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationProtocolGrade_IdValidationProtocol",
                table: "Training_ValidationProtocolGrade",
                column: "IdValidationProtocol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ConsultingClientRequiredDocument");

            migrationBuilder.DropTable(
                name: "Training_ValidationProtocolGrade");
        }
    }
}
