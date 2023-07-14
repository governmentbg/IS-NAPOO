using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTypeOfRequestedDocumentAddIdCourseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCourseType",
                table: "Request_TypeOfRequestedDocument",
                type: "int",
                nullable: true,
                comment: "Вид на курса за обучение, по който може да се получи типът документ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCourseType",
                table: "Request_TypeOfRequestedDocument");
        }
    }
}
