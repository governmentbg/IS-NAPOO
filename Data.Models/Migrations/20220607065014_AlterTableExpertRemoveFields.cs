using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableExpertRemoveFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpComm_Expert_Location_IdLocation",
                table: "ExpComm_Expert");

            migrationBuilder.DropIndex(
                name: "IX_ExpComm_Expert_IdLocation",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "FamilyName",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "IdIndentType",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "IdLocation",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "IdSex",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "Indent",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "PersonalID",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "PersonalIDIssueBy",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "ExpComm_Expert");

            migrationBuilder.DropColumn(
                name: "SecondName",
                table: "ExpComm_Expert");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ExpComm_Expert",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "ExpComm_Expert",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ExpComm_Expert",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FamilyName",
                table: "ExpComm_Expert",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ExpComm_Expert",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "IdIndentType",
                table: "ExpComm_Expert",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdLocation",
                table: "ExpComm_Expert",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdSex",
                table: "ExpComm_Expert",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Indent",
                table: "ExpComm_Expert",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalID",
                table: "ExpComm_Expert",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalIDIssueBy",
                table: "ExpComm_Expert",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ExpComm_Expert",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "ExpComm_Expert",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondName",
                table: "ExpComm_Expert",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_Expert_IdLocation",
                table: "ExpComm_Expert",
                column: "IdLocation");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpComm_Expert_Location_IdLocation",
                table: "ExpComm_Expert",
                column: "IdLocation",
                principalTable: "Location",
                principalColumn: "idLocation");
        }
    }
}
