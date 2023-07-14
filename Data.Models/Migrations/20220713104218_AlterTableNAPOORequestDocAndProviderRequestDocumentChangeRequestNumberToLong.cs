using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableNAPOORequestDocAndProviderRequestDocumentChangeRequestNumberToLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "RequestNumber",
                table: "Request_ProviderRequestDocument",
                type: "bigint",
                nullable: true,
                comment: "Ноемер на заявка",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Ноемер на заявка");

            migrationBuilder.AlterColumn<long>(
                name: "NAPOORequestNumber",
                table: "Request_NAPOORequestDoc",
                type: "bigint",
                nullable: true,
                comment: "Ноемер на заявка",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Ноемер на заявка");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RequestNumber",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: true,
                comment: "Ноемер на заявка",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Ноемер на заявка");

            migrationBuilder.AlterColumn<int>(
                name: "NAPOORequestNumber",
                table: "Request_NAPOORequestDoc",
                type: "int",
                nullable: true,
                comment: "Ноемер на заявка",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Ноемер на заявка");
        }
    }
}
