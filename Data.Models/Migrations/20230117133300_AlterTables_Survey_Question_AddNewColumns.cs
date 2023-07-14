using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_Survey_Question_AddNewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdSurveyТype",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Вид анкета",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Тип анкета");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Assess_Survey",
                type: "datetime2",
                nullable: true,
                comment: "Дата на активност до");

            migrationBuilder.AddColumn<int>(
                name: "IdSurveyTarget",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Тип анкета");

            migrationBuilder.AddColumn<int>(
                name: "IdTrainingCourseType",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Вид на курса за обучение");

            migrationBuilder.AddColumn<string>(
                name: "InternalCode",
                table: "Assess_Survey",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Вътрешен код на анкетата");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Assess_Survey",
                type: "datetime2",
                nullable: true,
                comment: "Дата на активност от");

            migrationBuilder.AddColumn<DateTime>(
                name: "TrainingPeriodFrom",
                table: "Assess_Survey",
                type: "datetime2",
                nullable: true,
                comment: "Период на обучение от");

            migrationBuilder.AddColumn<DateTime>(
                name: "TrainingPeriodTo",
                table: "Assess_Survey",
                type: "datetime2",
                nullable: true,
                comment: "Период на обучение до");

            migrationBuilder.AddColumn<int>(
                name: "AnswersCount",
                table: "Assess_Question",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Брой отговори към въпрос");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Assess_Question",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Поредност");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "IdSurveyTarget",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "IdTrainingCourseType",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "InternalCode",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "TrainingPeriodFrom",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "TrainingPeriodTo",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "AnswersCount",
                table: "Assess_Question");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "Assess_Question");

            migrationBuilder.AlterColumn<int>(
                name: "IdSurveyТype",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Тип анкета",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Вид анкета");
        }
    }
}
