using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableExpertNapoo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpComm_ExpertNapoo",
                columns: table => new
                {
                    IdExpertNapoo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdExpert = table.Column<int>(type: "int", nullable: false, comment: "Експерт"),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Длъжност"),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на назначаване"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус"),
                    Comment = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "История на промяната"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpComm_ExpertNapoo", x => x.IdExpertNapoo);
                    table.ForeignKey(
                        name: "FK_ExpComm_ExpertNapoo_ExpComm_Expert_IdExpert",
                        column: x => x.IdExpert,
                        principalTable: "ExpComm_Expert",
                        principalColumn: "IdExpert",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_ExpertNapoo_IdExpert",
                table: "ExpComm_ExpertNapoo",
                column: "IdExpert");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpComm_ExpertNapoo");
        }
    }
}
