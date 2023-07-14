using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablePayment_Add_ReceiptId_RegistrationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptId",
                table: "Procedure_Payment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Номер на заявка в pay.egov.bg");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationTime",
                table: "Procedure_Payment",
                type: "datetime2",
                nullable: true,
                comment: "Дата на заявка в pay.egov.bg");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptId",
                table: "Procedure_Payment");

            migrationBuilder.DropColumn(
                name: "RegistrationTime",
                table: "Procedure_Payment");
        }
    }
}
