using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_ValidationCompetency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_ValidationCompetency",
                columns: table => new
                {
                    IdValidationCompetency = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdValidationClient = table.Column<int>(type: "int", nullable: false, comment: "Връзка с обучаем от курс за валидиране"),
                    CompetencyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Номер на компетентност"),
                    Competency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Компетентност"),
                    IsCompetencyRecognized = table.Column<bool>(type: "bit", nullable: false, comment: "Дали се признава компетентността"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ValidationCompetency", x => x.IdValidationCompetency);
                    table.ForeignKey(
                        name: "FK_Training_ValidationCompetency_Training_ValidationClient_IdValidationClient",
                        column: x => x.IdValidationClient,
                        principalTable: "Training_ValidationClient",
                        principalColumn: "IdValidationClient",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Компетентност към курс за валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ValidationCompetency_IdValidationClient",
                table: "Training_ValidationCompetency",
                column: "IdValidationClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ValidationCompetency");
        }
    }
}
