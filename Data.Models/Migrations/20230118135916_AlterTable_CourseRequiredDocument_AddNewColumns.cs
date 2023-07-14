using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CourseRequiredDocument_AddNewColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Desciption",
                table: "Training_CourseRequiredDocument",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "DocumentPrnNo",
                table: "Training_CourseRequiredDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Фабричен номер");

            migrationBuilder.AddColumn<string>(
                name: "DocumentRegNo",
                table: "Training_CourseRequiredDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Регистрационен номер");

            migrationBuilder.AddColumn<int>(
                name: "IdClientCourse",
                table: "Training_CourseRequiredDocument",
                type: "int",
                nullable: true,
                comment: "Връзка с обучаем от курс за обучение");

            migrationBuilder.AddColumn<int>(
                name: "IdEducation",
                table: "Training_CourseRequiredDocument",
                type: "int",
                nullable: true,
                comment: "Завършено образование");

            migrationBuilder.AddColumn<int>(
                name: "IdMinimumQualificationLevel",
                table: "Training_CourseRequiredDocument",
                type: "int",
                nullable: true,
                comment: "Квалификационно ниво");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDocumentDate",
                table: "Training_CourseRequiredDocument",
                type: "datetime2",
                nullable: true,
                comment: "Дата на издаване на документа");

            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Training_CourseRequiredDocument",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Прикачен файл");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseRequiredDocument_IdClientCourse",
                table: "Training_CourseRequiredDocument",
                column: "IdClientCourse");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_CourseRequiredDocument_Training_ClientCourse_IdClientCourse",
                table: "Training_CourseRequiredDocument",
                column: "IdClientCourse",
                principalTable: "Training_ClientCourse",
                principalColumn: "IdClientCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_CourseRequiredDocument_Training_ClientCourse_IdClientCourse",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropIndex(
                name: "IX_Training_CourseRequiredDocument_IdClientCourse",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropColumn(
                name: "DocumentPrnNo",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropColumn(
                name: "DocumentRegNo",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropColumn(
                name: "IdClientCourse",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropColumn(
                name: "IdEducation",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropColumn(
                name: "IdMinimumQualificationLevel",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropColumn(
                name: "IssueDocumentDate",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Training_CourseRequiredDocument");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Training_CourseRequiredDocument",
                newName: "Desciption");
        }
    }
}
