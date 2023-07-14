using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableAspNetUsersAddPersonAlterTableNotificationReviewDateAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReviewDate",
                table: "Notification",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "IdPerson",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdPerson",
                table: "AspNetUsers",
                column: "IdPerson");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Person_IdPerson",
                table: "AspNetUsers",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "IdPerson");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Person_IdPerson",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IdPerson",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IdPerson",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReviewDate",
                table: "Notification",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
