using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableValidationClientDsColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DS_DATE",
                table: "Training_ValidationClient",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_DocNumber",
                table: "Training_ValidationClient",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_GUID",
                table: "Training_ValidationClient",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DS_ID",
                table: "Training_ValidationClient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_LINK",
                table: "Training_ValidationClient",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DS_OFFICIAL_DATE",
                table: "Training_ValidationClient",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_OFFICIAL_DocNumber",
                table: "Training_ValidationClient",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_OFFICIAL_GUID",
                table: "Training_ValidationClient",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DS_OFFICIAL_ID",
                table: "Training_ValidationClient",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_PREP",
                table: "Training_ValidationClient",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DS_DATE",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_DocNumber",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_GUID",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_ID",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_LINK",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_DATE",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_DocNumber",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_GUID",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_ID",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "DS_PREP",
                table: "Training_ValidationClient");
        }
    }
}
