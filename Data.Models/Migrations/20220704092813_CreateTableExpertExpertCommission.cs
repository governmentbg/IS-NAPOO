using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableExpertExpertCommission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpComm_ExpertExpertCommission",
                columns: table => new
                {
                    IdExpertExpertCommission = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdExpert = table.Column<int>(type: "int", nullable: false),
                    IdExpertCommission = table.Column<int>(type: "int", nullable: false, comment: "Eкспертна комисия - KeyTypeIntCode = ExpertCommission"),
                    IdRole = table.Column<int>(type: "int", nullable: false, comment: "Вид експерт"),
                    Institution = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Институция, която представя"),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Длъжност "),
                    Protokol = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Протокол"),
                    ProtokolDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на Протокол"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpComm_ExpertExpertCommission", x => x.IdExpertExpertCommission);
                    table.ForeignKey(
                        name: "FK_ExpComm_ExpertExpertCommission_ExpComm_Expert_IdExpert",
                        column: x => x.IdExpert,
                        principalTable: "ExpComm_Expert",
                        principalColumn: "IdExpert",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpComm_ExpertExpertCommission_IdExpert",
                table: "ExpComm_ExpertExpertCommission",
                column: "IdExpert");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpComm_ExpertExpertCommission");
        }
    }
}
