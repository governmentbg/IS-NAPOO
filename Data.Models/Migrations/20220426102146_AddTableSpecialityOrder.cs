using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTableSpecialityOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SPPOO_SpecialityOrder",
                columns: table => new
                {
                    IdSpecialityOrder = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSpeciality = table.Column<int>(type: "int", nullable: false),
                    IdSPPOOOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_SpecialityOrder", x => x.IdSpecialityOrder);
                    table.ForeignKey(
                        name: "FK_SPPOO_SpecialityOrder_SPPOO_Order_IdSPPOOOrder",
                        column: x => x.IdSPPOOOrder,
                        principalTable: "SPPOO_Order",
                        principalColumn: "IdOrder",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPPOO_SpecialityOrder_SPPOO_Speciality_IdSpeciality",
                        column: x => x.IdSpeciality,
                        principalTable: "SPPOO_Speciality",
                        principalColumn: "IdSpeciality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_SpecialityOrder_IdSpeciality",
                table: "SPPOO_SpecialityOrder",
                column: "IdSpeciality");

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_SpecialityOrder_IdSPPOOOrder",
                table: "SPPOO_SpecialityOrder",
                column: "IdSPPOOOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPPOO_SpecialityOrder");
        }
    }
}
