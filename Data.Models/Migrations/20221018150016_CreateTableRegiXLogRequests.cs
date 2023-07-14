using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableRegiXLogRequests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_RegiXLogRequest",
                columns: table => new
                {
                    IdRegiXLogRequest = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdministrationName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Наименование на администрацията, ползваща системата"),
                    AdministrationOId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false, comment: "Идентификационен код на администрация (oID от eAuth)"),
                    EmployeeIdentifier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Идентификатор на служител на администрацията"),
                    EmployeePosition = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Длъжност или позиция на служителя в администрацията"),
                    EmployeeNames = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Имена на служител на администрацията, иментата на служителя"),
                    ResponsiblePersonIdentifier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Опционален идентификатор на човека отговорен за справката."),
                    LawReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Контекст на правното основание"),
                    Remark = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Допълнително поле в свободен текст"),
                    ServiceType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Вид на услугата, във връзка с която се извиква операцията"),
                    ServiceURI = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Идентификатор на инстанцията на административната услуга или процедура в администрацията (например: номер на преписка)"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_RegiXLogRequest", x => x.IdRegiXLogRequest);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_RegiXLogRequest");
        }
    }
}
