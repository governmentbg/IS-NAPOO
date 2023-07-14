using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientCourseDocument_AddColumn_FinalResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinalResult",
                table: "Training_ClientCourseDocument",
                type: "decimal(3,2)",
                nullable: true,
                comment: "Обща оценка от теория и практика");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalResult",
                table: "Training_ClientCourseDocument");
        }
    }
}
