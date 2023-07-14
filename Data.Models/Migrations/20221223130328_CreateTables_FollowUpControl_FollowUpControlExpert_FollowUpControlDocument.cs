using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTables_FollowUpControl_FollowUpControlExpert_FollowUpControlDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Control_FollowUpControl",
                columns: table => new
                {
                    IdFollowUpControl = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidateProvider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с проверяван ЦПО/ЦИПО"),
                    IdFollowUpControlType = table.Column<int>(type: "int", nullable: false, comment: "Връзка с номенклатура за вид на последващия контрол"),
                    IdControlType = table.Column<int>(type: "int", nullable: false, comment: "Връзка с номенклатура за вид на проверката"),
                    ControlStartDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Срок на проверката от"),
                    ControlEndDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Срок на проверката до"),
                    IsFollowUpControlOnline = table.Column<bool>(type: "bit", nullable: false, comment: "По документи в ИС на НАПОО и въз основа на допълнително изискани документи - чек бокс"),
                    IsFollowUpControlOnsite = table.Column<bool>(type: "bit", nullable: false, comment: "Последващият контрол се извършва на място"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control_FollowUpControl", x => x.IdFollowUpControl);
                    table.ForeignKey(
                        name: "FK_Control_FollowUpControl_Candidate_Provider_IdCandidateProvider",
                        column: x => x.IdCandidateProvider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Последващ контрол, изпълняван от служител/и на НАПОО");

            migrationBuilder.CreateTable(
                name: "Control_FollowUpControlDocument",
                columns: table => new
                {
                    IdFollowUpControlDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFollowUpControl = table.Column<int>(type: "int", nullable: false, comment: "Връзка с последващ контрол"),
                    IdDocumentType = table.Column<int>(type: "int", nullable: true, comment: "Тип на документа при последващ контрол"),
                    DS_ID = table.Column<int>(type: "int", nullable: true),
                    DS_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_GUID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DS_DocNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_OFFICIAL_ID = table.Column<int>(type: "int", nullable: true),
                    DS_OFFICIAL_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DS_OFFICIAL_GUID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DS_OFFICIAL_DocNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DS_PREP = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DS_LINK = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control_FollowUpControlDocument", x => x.IdFollowUpControlDocument);
                    table.ForeignKey(
                        name: "FK_Control_FollowUpControlDocument_Control_FollowUpControl_IdFollowUpControl",
                        column: x => x.IdFollowUpControl,
                        principalTable: "Control_FollowUpControl",
                        principalColumn: "IdFollowUpControl",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Протокол/доклад/заповед във връзка с проследяващ контрол");

            migrationBuilder.CreateTable(
                name: "Control_FollowUpControlExpert",
                columns: table => new
                {
                    IdFollowUpControlExpert = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFollowUpControl = table.Column<int>(type: "int", nullable: false, comment: "Връзка с проследяващ контрол"),
                    IdExpert = table.Column<int>(type: "int", nullable: false, comment: "Връзка с експерт на НАПОО"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Control_FollowUpControlExpert", x => x.IdFollowUpControlExpert);
                    table.ForeignKey(
                        name: "FK_Control_FollowUpControlExpert_Control_FollowUpControl_IdFollowUpControl",
                        column: x => x.IdFollowUpControl,
                        principalTable: "Control_FollowUpControl",
                        principalColumn: "IdFollowUpControl",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Control_FollowUpControlExpert_ExpComm_Expert_IdExpert",
                        column: x => x.IdExpert,
                        principalTable: "ExpComm_Expert",
                        principalColumn: "IdExpert",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Връзка на проследяващ контрол с експерт на НАПОО");

            migrationBuilder.CreateIndex(
                name: "IX_Control_FollowUpControl_IdCandidateProvider",
                table: "Control_FollowUpControl",
                column: "IdCandidateProvider");

            migrationBuilder.CreateIndex(
                name: "IX_Control_FollowUpControlDocument_IdFollowUpControl",
                table: "Control_FollowUpControlDocument",
                column: "IdFollowUpControl");

            migrationBuilder.CreateIndex(
                name: "IX_Control_FollowUpControlExpert_IdExpert",
                table: "Control_FollowUpControlExpert",
                column: "IdExpert");

            migrationBuilder.CreateIndex(
                name: "IX_Control_FollowUpControlExpert_IdFollowUpControl",
                table: "Control_FollowUpControlExpert",
                column: "IdFollowUpControl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Control_FollowUpControlDocument");

            migrationBuilder.DropTable(
                name: "Control_FollowUpControlExpert");

            migrationBuilder.DropTable(
                name: "Control_FollowUpControl");
        }
    }
}
