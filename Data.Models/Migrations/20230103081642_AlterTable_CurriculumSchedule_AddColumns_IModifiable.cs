using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CurriculumSchedule_AddColumns_IModifiable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Training_CurriculumSchedule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "Training_CurriculumSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "Training_CurriculumSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Training_CurriculumSchedule",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Training_CurriculumSchedule");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "Training_CurriculumSchedule");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "Training_CurriculumSchedule");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Training_CurriculumSchedule");
        }
    }
}
