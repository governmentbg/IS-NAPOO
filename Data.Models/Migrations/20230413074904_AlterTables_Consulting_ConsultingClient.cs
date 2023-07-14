using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTables_Consulting_ConsultingClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IdCityOfBirth",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IdCountryOfBirth",
                table: "Training_ConsultingClient");

            migrationBuilder.AddColumn<int>(
                name: "IdAimAtCIPOServicesType",
                table: "Training_ConsultingClient",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Вид на насочен към услугите на ЦИПО");

            migrationBuilder.AddColumn<int>(
                name: "IdRegistrationAtLabourOfficeType",
                table: "Training_ConsultingClient",
                type: "int",
                nullable: true,
                comment: "Вид на регистрация в бюрото по труда");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmployedPerson",
                table: "Training_ConsultingClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Заето лице");

            migrationBuilder.AddColumn<bool>(
                name: "IsStudent",
                table: "Training_ConsultingClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Учащ");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Training_Consulting",
                type: "decimal(10,2)",
                nullable: true,
                comment: "Цена (в лева за консултирано лице)");

            migrationBuilder.AddColumn<int>(
                name: "IdConsultingReceiveType",
                table: "Training_Consulting",
                type: "int",
                nullable: true,
                comment: "Начин на предоставяне на услугата");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdAimAtCIPOServicesType",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IdRegistrationAtLabourOfficeType",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IsEmployedPerson",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "IsStudent",
                table: "Training_ConsultingClient");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Training_Consulting");

            migrationBuilder.DropColumn(
                name: "IdConsultingReceiveType",
                table: "Training_Consulting");

            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "Training_ConsultingClient",
                type: "decimal(10,2)",
                nullable: true,
                comment: "Цена (в лева за консултирано лице)");

            migrationBuilder.AddColumn<int>(
                name: "IdCityOfBirth",
                table: "Training_ConsultingClient",
                type: "int",
                nullable: true,
                comment: "Месторождение (населено място)");

            migrationBuilder.AddColumn<int>(
                name: "IdCountryOfBirth",
                table: "Training_ConsultingClient",
                type: "int",
                nullable: true,
                comment: "Месторождение (държава)");
        }
    }
}
