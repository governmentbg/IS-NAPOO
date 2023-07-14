using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableRequestDocumentManagementIdDocumentRequestReceiveTypeAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdDocumentRequestReceiveType",
                table: "Request_RequestDocumentManagement",
                type: "int",
                nullable: true,
                comment: "Начин на получаване на заявката за документи",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Начин на получаване на заявката за документи");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdDocumentRequestReceiveType",
                table: "Request_RequestDocumentManagement",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Начин на получаване на заявката за документи",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Начин на получаване на заявката за документи");
        }
    }
}
