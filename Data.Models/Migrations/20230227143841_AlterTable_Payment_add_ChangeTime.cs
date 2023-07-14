using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Payment_add_ChangeTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangeTime",
                table: "Procedure_Payment",
                type: "datetime2",
                nullable: true,
                comment: "Време на промяна на статуса на заявката за плащане");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeTime",
                table: "Procedure_Payment");
        }
    }
}
