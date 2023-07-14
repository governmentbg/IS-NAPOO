using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableEventLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventLog",
                columns: table => new
                {
                    idEventLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idUser = table.Column<int>(type: "int", nullable: true, comment: "Потребител направил завката"),
                    idAspNetUsers = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Потребител направил завката"),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата час на действието"),
                    EventMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Допълнителна информация"),
                    EventAction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Вид събитие - UPDATE, INSERT, DELETE"),
                    EntityID = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Oбект, над който е извършено действието"),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "EntityName - таблицата"),
                    PersonName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Име на потребителя напавил действието"),
                    IP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "IP адрес"),
                    BrowserInformation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Браузър на потребителя")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventLog", x => x.idEventLog);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventLog");
        }
    }
}
