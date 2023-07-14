using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableNegativeIssueAlterTableApplicationRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "AspNetRoles",
                newName: "ModifyDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "AspNetRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Procedure_NegativeIssue",
                columns: table => new
                {
                    IdNegativeIssue = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdStartedProcedure = table.Column<int>(type: "int", nullable: false),
                    NegativeIssueText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_NegativeIssue", x => x.IdNegativeIssue);
                    table.ForeignKey(
                        name: "FK_Procedure_NegativeIssue_Procedure_StartedProcedure_IdStartedProcedure",
                        column: x => x.IdStartedProcedure,
                        principalTable: "Procedure_StartedProcedure",
                        principalColumn: "IdStartedProcedure",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_NegativeIssue_IdStartedProcedure",
                table: "Procedure_NegativeIssue",
                column: "IdStartedProcedure");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedure_NegativeIssue");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "AspNetRoles");

            migrationBuilder.RenameColumn(
                name: "ModifyDate",
                table: "AspNetRoles",
                newName: "CreatedOn");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "AspNetRoles",
                type: "datetime2",
                nullable: true);
        }
    }
}
