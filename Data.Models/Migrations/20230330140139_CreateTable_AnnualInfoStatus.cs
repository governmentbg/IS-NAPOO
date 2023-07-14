using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_AnnualInfoStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "Arch_AnnualInfo",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Статус на отчета за годишна информация");

            migrationBuilder.CreateTable(
                name: "Arch_AnnualInfoStatus",
                columns: table => new
                {
                    IdAnnualInfoStatus = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAnnualInfo = table.Column<int>(type: "int", nullable: false, comment: "Връзка с отчета за годишна информация"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус на отчета за годишна информация"),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Коментар при операция с отчета за годишна информация"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_AnnualInfoStatus", x => x.IdAnnualInfoStatus);
                    table.ForeignKey(
                        name: "FK_Arch_AnnualInfoStatus_Arch_AnnualInfo_IdAnnualInfo",
                        column: x => x.IdAnnualInfo,
                        principalTable: "Arch_AnnualInfo",
                        principalColumn: "IdAnnualInfo",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arch_AnnualInfoStatus_IdAnnualInfo",
                table: "Arch_AnnualInfoStatus",
                column: "IdAnnualInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_AnnualInfoStatus");

            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "Arch_AnnualInfo");
        }
    }
}
