using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_FollowUpControl_AddColumn_OnsiteControlDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OnsiteControlDate",
                table: "Control_FollowUpControl",
                type: "datetime2",
                nullable: true,
                comment: "Дата на последващ контрол, само ако се провежда на място");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnsiteControlDate",
                table: "Control_FollowUpControl");
        }
    }
}
