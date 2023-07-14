using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_ClientCourse_ValidaitonClient_ConsultingClient_AddTwoColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabledPerson",
                table: "Training_ValidationClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Лице с увреждания");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisadvantagedPerson",
                table: "Training_ValidationClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Лице в неравностойно положение");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabledPerson",
                table: "Training_ConsultingClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Лице с увреждания");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisadvantagedPerson",
                table: "Training_ConsultingClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Лице в неравностойно положение");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisabledPerson",
                table: "Training_ClientCourse",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Лице с увреждания");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisadvantagedPerson",
                table: "Training_ClientCourse",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Лице в неравностойно положение");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabledPerson",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "IsDisadvantagedPerson",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "IsDisabledPerson",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IsDisadvantagedPerson",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IsDisabledPerson",
                table: "Training_ClientCourse");

            migrationBuilder.DropColumn(
                name: "IsDisadvantagedPerson",
                table: "Training_ClientCourse");
        }
    }
}
