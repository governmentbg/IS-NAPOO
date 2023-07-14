using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableCandidateProviderLicenceChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Candidate_ProviderLicenceChange",
                columns: table => new
                {
                    IdCandidateProviderLicenceChange = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Смяна на лицензията/Статус"),
                    ChangeDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на промяна"),
                    NumberCommand = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Номер на заповедта"),
                    Notes = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "Бележки"),
                    IdLicenceStatusDetail = table.Column<int>(type: "int", nullable: true, comment: "Детайли при смяна на лицензията/Статус"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidate_ProviderLicenceChange", x => x.IdCandidateProviderLicenceChange);
                    table.ForeignKey(
                        name: "FK_Candidate_ProviderLicenceChange_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderLicenceChange_IdCandidate_Provider",
                table: "Candidate_ProviderLicenceChange",
                column: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidate_ProviderLicenceChange");
        }
    }
}
