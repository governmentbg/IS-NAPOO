using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientCourseAddColumn_CourseJoinDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CourseJoinDate",
                table: "Training_ClientCourse",
                type: "datetime2",
                nullable: true,
                comment: "Дата на включване в курса");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseJoinDate",
                table: "Training_ClientCourse");
        }
    }
}
