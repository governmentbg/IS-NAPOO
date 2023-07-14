using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableAnnualInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_AnnualInfo",
                columns: table => new
                {
                    IdAnnualInfo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с CPO,CIPO - Обучаваща институция"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Име на лица подало годишната информация"),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Длъжност"),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Телефон"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "E-mail"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_AnnualInfo", x => x.IdAnnualInfo);
                    table.ForeignKey(
                        name: "FK_Arch_AnnualInfo_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Arch_AnnualInfo_IdCandidateProvider",
                table: "Arch_AnnualInfo",
                column: "IdCandidateProvider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_AnnualInfo");
        }
    }
}
