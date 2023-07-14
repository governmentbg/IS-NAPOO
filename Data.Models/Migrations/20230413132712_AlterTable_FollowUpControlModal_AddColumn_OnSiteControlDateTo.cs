using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_FollowUpControlModal_AddColumn_OnSiteControlDateTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnsiteControlDate",
                table: "Control_FollowUpControl");

            migrationBuilder.AddColumn<DateTime>(
                name: "OnsiteControlDateFrom",
                table: "Control_FollowUpControl",
                type: "datetime2",
                nullable: true,
                comment: "Дата на последващ контрол от, само ако се провежда на място");

            migrationBuilder.AddColumn<DateTime>(
                name: "OnsiteControlDateTo",
                table: "Control_FollowUpControl",
                type: "datetime2",
                nullable: true,
                comment: "Дата на последващ контрол до, само ако се провежда на място");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnsiteControlDateFrom",
                table: "Control_FollowUpControl");

            migrationBuilder.DropColumn(
                name: "OnsiteControlDateTo",
                table: "Control_FollowUpControl");

            migrationBuilder.AddColumn<DateTime>(
                name: "OnsiteControlDate",
                table: "Control_FollowUpControl",
                type: "datetime2",
                nullable: true,
                comment: "Дата на последващ контрол, само ако се провежда на място");
        }
    }
}
