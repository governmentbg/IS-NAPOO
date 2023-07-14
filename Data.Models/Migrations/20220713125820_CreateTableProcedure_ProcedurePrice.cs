using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableProcedure_ProcedurePrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Procedure_ProcedurePrice",
                columns: table => new
                {
                    IdProcedurePrice = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdTypeApplication = table.Column<int>(type: "int", nullable: false),
                    CountProfessionsFrom = table.Column<int>(type: "int", nullable: true),
                    CountProfessionsTo = table.Column<int>(type: "int", nullable: true),
                    ExpirationDateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDateTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdApplicationStatus = table.Column<int>(type: "int", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedure_ProcedurePrice", x => x.IdProcedurePrice);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Procedure_ProcedurePrice");
        }
    }
}
