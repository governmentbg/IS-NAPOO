using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_FollowUpContro_AddColumn_IdDeadlinePeriodType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "idAspNetUsers",
                table: "EventLog",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Потребител направил завката",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Потребител направил завката");

            migrationBuilder.AddColumn<int>(
                name: "IdDeadlinePeriodType",
                table: "Control_FollowUpControl",
                type: "int",
                nullable: true,
                comment: "Срок за отстраняване на констатираните нередовности/нарушения");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdDeadlinePeriodType",
                table: "Control_FollowUpControl");

            migrationBuilder.AlterColumn<string>(
                name: "idAspNetUsers",
                table: "EventLog",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Потребител направил завката",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Потребител направил завката");
        }
    }
}
