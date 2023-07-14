using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_FollowUpControlUploadedFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodFrom",
                table: "Control_FollowUpControl",
                type: "datetime2",
                nullable: true,
                comment: "Период от");

            migrationBuilder.AddColumn<DateTime>(
                name: "PeriodTo",
                table: "Control_FollowUpControl",
                type: "datetime2",
                nullable: true,
                comment: "Период до");

            migrationBuilder.CreateTable(
                name: "Control_FollowUpControlUploadedFile",
                columns: table => new
                {
                    IdFollowUpControlUploadedFile = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFollowUpControl = table.Column<int>(type: "int", nullable: false, comment: "Връзка с последващ контрол"),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Описание на прикачения файл"),
                    UploadedFileName = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Прикачен файл"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control_FollowUpControlUploadedFile", x => x.IdFollowUpControlUploadedFile);
                    table.ForeignKey(
                        name: "FK_Control_FollowUpControlUploadedFile_Control_FollowUpControl_IdFollowUpControl",
                        column: x => x.IdFollowUpControl,
                        principalTable: "Control_FollowUpControl",
                        principalColumn: "IdFollowUpControl",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Прикачен файл към последващ контрол");

            migrationBuilder.CreateIndex(
                name: "IX_Control_FollowUpControlUploadedFile_IdFollowUpControl",
                table: "Control_FollowUpControlUploadedFile",
                column: "IdFollowUpControl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Control_FollowUpControlUploadedFile");

            migrationBuilder.DropColumn(
                name: "PeriodFrom",
                table: "Control_FollowUpControl");

            migrationBuilder.DropColumn(
                name: "PeriodTo",
                table: "Control_FollowUpControl");
        }
    }
}
