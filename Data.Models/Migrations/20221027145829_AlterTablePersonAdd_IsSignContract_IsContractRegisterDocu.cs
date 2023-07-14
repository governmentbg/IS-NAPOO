using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablePersonAdd_IsSignContract_IsContractRegisterDocu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsContractRegisterDocu",
                table: "Person",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Договорът се регистрира в деловодната система");

            migrationBuilder.AddColumn<bool>(
                name: "IsSignContract",
                table: "Person",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Сключва се договор");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsContractRegisterDocu",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "IsSignContract",
                table: "Person");
        }
    }
}
