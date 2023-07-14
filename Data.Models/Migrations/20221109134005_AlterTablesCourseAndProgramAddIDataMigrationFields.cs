using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesCourseAndProgramAddIDataMigrationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_Program",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_Program",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_Course",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_Course",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_Program");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_Program");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_Course");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_Course");
        }
    }
}
