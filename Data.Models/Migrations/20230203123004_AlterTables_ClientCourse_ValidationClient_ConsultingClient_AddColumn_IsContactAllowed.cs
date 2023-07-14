using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_ClientCourse_ValidationClient_ConsultingClient_AddColumn_IsContactAllowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsContactAllowed",
                table: "Training_ValidationClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Съгласие за използване на информацията за контакт от НАПОО");

            migrationBuilder.AddColumn<bool>(
                name: "IsContactAllowed",
                table: "Training_ConsultingClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Съгласие за използване на информацията за контакт от НАПОО");

            migrationBuilder.AddColumn<bool>(
                name: "IsContactAllowed",
                table: "Training_ClientCourse",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Съгласие за използване на информацията за контакт от НАПОО");

            migrationBuilder.AddColumn<int>(
                name: "IdLegalCapacityOrdinanceType",
                table: "SPPOO_Profession",
                type: "int",
                nullable: true,
                comment: "Вид на наредбата за правоспособност");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsContactAllowed",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "IsContactAllowed",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IsContactAllowed",
                table: "Training_ClientCourse");

            migrationBuilder.DropColumn(
                name: "IdLegalCapacityOrdinanceType",
                table: "SPPOO_Profession");
        }
    }
}
