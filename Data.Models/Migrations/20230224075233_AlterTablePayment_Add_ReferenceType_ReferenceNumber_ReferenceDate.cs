using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablePayment_Add_ReferenceType_ReferenceNumber_ReferenceDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInformation",
                table: "Procedure_Payment",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Допълнителна информация",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldComment: "Допълнителна информация");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReferenceDate",
                table: "Procedure_Payment",
                type: "datetime2",
                nullable: true,
                comment: "Дата на документ (референтен документ за плащане)");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "Procedure_Payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Номер на документ (референтен документ за\r\nплащане");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "Procedure_Payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Тип на документ (референтен документ за плащане)");

            migrationBuilder.AddColumn<string>(
                name: "ServiceProviderName",
                table: "Procedure_Payment",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Доставчик на ЕАУ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceDate",
                table: "Procedure_Payment");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "Procedure_Payment");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "Procedure_Payment");

            migrationBuilder.DropColumn(
                name: "ServiceProviderName",
                table: "Procedure_Payment");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalInformation",
                table: "Procedure_Payment",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Допълнителна информация",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "Допълнителна информация");
        }
    }
}
