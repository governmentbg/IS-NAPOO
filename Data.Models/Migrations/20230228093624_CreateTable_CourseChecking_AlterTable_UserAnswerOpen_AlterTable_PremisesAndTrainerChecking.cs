using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_CourseChecking_AlterTable_UserAnswerOpen_AlterTable_PremisesAndTrainerChecking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Assess_UserAnswerOpen");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "Assess_UserAnswerOpen");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "Assess_UserAnswerOpen");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Assess_UserAnswerOpen");

            migrationBuilder.AddColumn<int>(
                name: "IdFollowUpControl",
                table: "Candidate_ProviderTrainerChecking",
                type: "int",
                nullable: true,
                comment: "Последващ контрол, изпълняван от служител/и на НАПОО");

            migrationBuilder.AddColumn<int>(
                name: "IdFollowUpControl",
                table: "Candidate_ProviderPremisesChecking",
                type: "int",
                nullable: true,
                comment: "Последващ контрол, изпълняван от служител/и на НАПОО");

            migrationBuilder.CreateTable(
                name: "Training_CourseChecking",
                columns: table => new
                {
                    IdCourseChecking = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс за обучение"),
                    IdFollowUpControl = table.Column<int>(type: "int", nullable: true, comment: "Последващ контрол, изпълняван от служител/и на НАПОО"),
                    CheckDone = table.Column<bool>(type: "bit", nullable: false, comment: "Извършена проверка от експерт на НАПОО"),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Коментар"),
                    CheckingDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на проверка"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseChecking", x => x.IdCourseChecking);
                    table.ForeignKey(
                        name: "FK_Training_CourseChecking_Control_FollowUpControl_IdFollowUpControl",
                        column: x => x.IdFollowUpControl,
                        principalTable: "Control_FollowUpControl",
                        principalColumn: "IdFollowUpControl");
                    table.ForeignKey(
                        name: "FK_Training_CourseChecking_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderTrainerChecking_IdFollowUpControl",
                table: "Candidate_ProviderTrainerChecking",
                column: "IdFollowUpControl");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderPremisesChecking_IdFollowUpControl",
                table: "Candidate_ProviderPremisesChecking",
                column: "IdFollowUpControl");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseChecking_IdCourse",
                table: "Training_CourseChecking",
                column: "IdCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseChecking_IdFollowUpControl",
                table: "Training_CourseChecking",
                column: "IdFollowUpControl");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_ProviderPremisesChecking_Control_FollowUpControl_IdFollowUpControl",
                table: "Candidate_ProviderPremisesChecking",
                column: "IdFollowUpControl",
                principalTable: "Control_FollowUpControl",
                principalColumn: "IdFollowUpControl");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_ProviderTrainerChecking_Control_FollowUpControl_IdFollowUpControl",
                table: "Candidate_ProviderTrainerChecking",
                column: "IdFollowUpControl",
                principalTable: "Control_FollowUpControl",
                principalColumn: "IdFollowUpControl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_ProviderPremisesChecking_Control_FollowUpControl_IdFollowUpControl",
                table: "Candidate_ProviderPremisesChecking");

            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_ProviderTrainerChecking_Control_FollowUpControl_IdFollowUpControl",
                table: "Candidate_ProviderTrainerChecking");

            migrationBuilder.DropTable(
                name: "Training_CourseChecking");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_ProviderTrainerChecking_IdFollowUpControl",
                table: "Candidate_ProviderTrainerChecking");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_ProviderPremisesChecking_IdFollowUpControl",
                table: "Candidate_ProviderPremisesChecking");

            migrationBuilder.DropColumn(
                name: "IdFollowUpControl",
                table: "Candidate_ProviderTrainerChecking");

            migrationBuilder.DropColumn(
                name: "IdFollowUpControl",
                table: "Candidate_ProviderPremisesChecking");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Assess_UserAnswerOpen",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "Assess_UserAnswerOpen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "Assess_UserAnswerOpen",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Assess_UserAnswerOpen",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
