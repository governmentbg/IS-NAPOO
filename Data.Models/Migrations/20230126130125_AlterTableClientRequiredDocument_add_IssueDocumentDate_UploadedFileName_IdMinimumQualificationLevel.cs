using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientRequiredDocument_add_IssueDocumentDate_UploadedFileName_IdMinimumQualificationLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdMinimumQualificationLevel",
                table: "Training_ClientRequiredDocument",
                type: "int",
                nullable: true,
                comment: "Квалификационно ниво");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDocumentDate",
                table: "Training_ClientRequiredDocument",
                type: "datetime2",
                nullable: true,
                comment: "Дата на издаване на документа");

            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Training_ClientRequiredDocument",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Прикачен файл");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdMinimumQualificationLevel",
                table: "Training_ClientRequiredDocument");

            migrationBuilder.DropColumn(
                name: "IssueDocumentDate",
                table: "Training_ClientRequiredDocument");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Training_ClientRequiredDocument");
        }
    }
}
