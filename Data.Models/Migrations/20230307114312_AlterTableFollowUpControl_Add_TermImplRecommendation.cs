using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableFollowUpControl_Add_TermImplRecommendation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TermImplRecommendation",
                table: "Control_FollowUpControl",
                type: "datetime2",
                nullable: true,
                comment: "Срок за изпълнение на препоръки");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermImplRecommendation",
                table: "Control_FollowUpControl");
        }
    }
}
