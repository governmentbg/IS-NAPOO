using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class addTemplateDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateDocument",
                columns: table => new
                {
                    idTemplateDocument = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TemplatePath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    IdStatus = table.Column<int>(type: "int", nullable: false),
                    IdModule = table.Column<int>(type: "int", nullable: false),
                    IdApplicationType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateDocument", x => x.idTemplateDocument);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateDocument");
        }
    }
}
