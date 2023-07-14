using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableFrameworkProgramAdd_IsValid_Description_ShortDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SPPOO_FrameworkProgram",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Описание");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "SPPOO_FrameworkProgram",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Валидно ли е");

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "SPPOO_FrameworkProgram",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "SPPOO_FrameworkProgram",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortDescription",
                table: "SPPOO_FrameworkProgram",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "Кратко описание");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.DropColumn(
                name: "ShortDescription",
                table: "SPPOO_FrameworkProgram");
        }
    }
}
