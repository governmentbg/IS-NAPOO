using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class createTableFrameworkProgramFormEducations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdFormEducation",
                table: "SPPOO_FrameworkProgram");

            migrationBuilder.CreateTable(
                name: "SPPOO_FrameworkProgramFormEducation",
                columns: table => new
                {
                    IdFrameworkProgramFormEducation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdFrameworkProgram = table.Column<int>(type: "int", nullable: false),
                    IdFormEducation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_FrameworkProgramFormEducation", x => x.IdFrameworkProgramFormEducation);
                    table.ForeignKey(
                        name: "FK_SPPOO_FrameworkProgramFormEducation_SPPOO_FrameworkProgram_IdFrameworkProgram",
                        column: x => x.IdFrameworkProgram,
                        principalTable: "SPPOO_FrameworkProgram",
                        principalColumn: "IdFrameworkProgram",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_FrameworkProgramFormEducation_IdFrameworkProgram",
                table: "SPPOO_FrameworkProgramFormEducation",
                column: "IdFrameworkProgram");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPPOO_FrameworkProgramFormEducation");

            migrationBuilder.AddColumn<int>(
                name: "IdFormEducation",
                table: "SPPOO_FrameworkProgram",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
