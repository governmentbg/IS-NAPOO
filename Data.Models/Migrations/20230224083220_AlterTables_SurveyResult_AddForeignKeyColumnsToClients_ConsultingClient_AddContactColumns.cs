using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_SurveyResult_AddForeignKeyColumnsToClients_ConsultingClient_AddContactColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Training_ConsultingClient",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Адрес");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Training_ConsultingClient",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "E-mail адрес");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Training_ConsultingClient",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Телефон");

            migrationBuilder.AddColumn<int>(
                name: "IdClientCourse",
                table: "Assess_SurveyResult",
                type: "int",
                nullable: true,
                comment: "Връзка с обучаем от курс за обучение");

            migrationBuilder.AddColumn<int>(
                name: "IdConsultingClient",
                table: "Assess_SurveyResult",
                type: "int",
                nullable: true,
                comment: "Връзка с консултирано лице");

            migrationBuilder.AddColumn<int>(
                name: "IdValidationClient",
                table: "Assess_SurveyResult",
                type: "int",
                nullable: true,
                comment: "Връзка с обучаем от курс за валидиране");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_SurveyResult_IdClientCourse",
                table: "Assess_SurveyResult",
                column: "IdClientCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_SurveyResult_IdConsultingClient",
                table: "Assess_SurveyResult",
                column: "IdConsultingClient");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_SurveyResult_IdValidationClient",
                table: "Assess_SurveyResult",
                column: "IdValidationClient");

            migrationBuilder.AddForeignKey(
                name: "FK_Assess_SurveyResult_Training_ClientCourse_IdClientCourse",
                table: "Assess_SurveyResult",
                column: "IdClientCourse",
                principalTable: "Training_ClientCourse",
                principalColumn: "IdClientCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_Assess_SurveyResult_Training_ConsultingClient_IdConsultingClient",
                table: "Assess_SurveyResult",
                column: "IdConsultingClient",
                principalTable: "Training_ConsultingClient",
                principalColumn: "IdConsultingClient");

            migrationBuilder.AddForeignKey(
                name: "FK_Assess_SurveyResult_Training_ValidationClient_IdValidationClient",
                table: "Assess_SurveyResult",
                column: "IdValidationClient",
                principalTable: "Training_ValidationClient",
                principalColumn: "IdValidationClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assess_SurveyResult_Training_ClientCourse_IdClientCourse",
                table: "Assess_SurveyResult");

            migrationBuilder.DropForeignKey(
                name: "FK_Assess_SurveyResult_Training_ConsultingClient_IdConsultingClient",
                table: "Assess_SurveyResult");

            migrationBuilder.DropForeignKey(
                name: "FK_Assess_SurveyResult_Training_ValidationClient_IdValidationClient",
                table: "Assess_SurveyResult");

            migrationBuilder.DropIndex(
                name: "IX_Assess_SurveyResult_IdClientCourse",
                table: "Assess_SurveyResult");

            migrationBuilder.DropIndex(
                name: "IX_Assess_SurveyResult_IdConsultingClient",
                table: "Assess_SurveyResult");

            migrationBuilder.DropIndex(
                name: "IX_Assess_SurveyResult_IdValidationClient",
                table: "Assess_SurveyResult");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IdClientCourse",
                table: "Assess_SurveyResult");

            migrationBuilder.DropColumn(
                name: "IdConsultingClient",
                table: "Assess_SurveyResult");

            migrationBuilder.DropColumn(
                name: "IdValidationClient",
                table: "Assess_SurveyResult");
        }
    }
}
