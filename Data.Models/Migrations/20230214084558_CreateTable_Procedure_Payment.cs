using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_Procedure_Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procedure_Payment",
                columns: table => new
                {
                    IdPayment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  CPO,CIPO - Кандидат Обучаваща институция"),
                    IdProcedurePrice = table.Column<int>(type: "int", nullable: false, comment: "Такси за лицензиране"),
                    IdPaymentStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус на плащане"),
                    ServiceProviderBank = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Име на банката, в която е сметката на доставчика на ЕАУ"),
                    ServiceProviderBIC = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "BIC код на сметката на доставчика на ЕАУ"),
                    ServiceProviderIBAN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "IBAN код на сметката на доставчика на ЕАУ"),
                    Currency = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false, comment: "Валута в която се плаща задължението (три символа, пр. \"BGN\")"),
                    PaymentTypeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Код на плащане"),
                    PaymentAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: true, comment: "Сума на задължението (десетичен разделител \".\", до 2 символа след десетичния разделител, пр. \"2.33\")"),
                    PaymentReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Основание за плащане"),
                    ApplicantUinTypeId = table.Column<int>(type: "int", nullable: false, comment: "тип на идентификатора на задължено лице (\"1\", \"2\" или \"3\" -> ЕГН = 1, ЛНЧ = 2, БУЛСТАТ = 3)"),
                    ApplicantUin = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Идентификатор на задължено лице"),
                    ApplicantName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Име на задължено лице"),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на изтичане на заявката за плащане"),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false, comment: "Допълнителна информация"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_Payment", x => x.IdPayment);
                    table.ForeignKey(
                        name: "FK_Procedure_Payment_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedure_Payment_Procedure_ProcedurePrice_IdProcedurePrice",
                        column: x => x.IdProcedurePrice,
                        principalTable: "Procedure_ProcedurePrice",
                        principalColumn: "IdProcedurePrice",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_Payment_IdCandidate_Provider",
                table: "Procedure_Payment",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Procedure_Payment_IdProcedurePrice",
                table: "Procedure_Payment",
                column: "IdProcedurePrice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedure_Payment");
        }
    }
}
