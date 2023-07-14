using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableAnnualInfoColumnNameLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Arch_AnnualInfo",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                comment: "Име на лица подало годишната информация",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldComment: "Име на лица подало годишната информация");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Arch_AnnualInfo",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                comment: "Име на лица подало годишната информация",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldComment: "Име на лица подало годишната информация");
        }
    }
}
