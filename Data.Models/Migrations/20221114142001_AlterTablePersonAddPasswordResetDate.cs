using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablePersonAddPasswordResetDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetDate",
                table: "Person",
                type: "datetime2",
                nullable: true,
                comment: "Дата на нулиране на парола");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetDate",
                table: "Person");
        }
    }
}
