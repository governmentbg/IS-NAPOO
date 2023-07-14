using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTablesProfessionOrderProfessionalDirectionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SPPOO_ProfessionalDirectionOrder",
                columns: table => new
                {
                    IdProfessionalDirectionOrder = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: false),
                    IdSPPOOOrder = table.Column<int>(type: "int", nullable: false),
                    IdTypeChange = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_ProfessionalDirectionOrder", x => x.IdProfessionalDirectionOrder);
                    table.ForeignKey(
                        name: "FK_SPPOO_ProfessionalDirectionOrder_SPPOO_Order_IdSPPOOOrder",
                        column: x => x.IdSPPOOOrder,
                        principalTable: "SPPOO_Order",
                        principalColumn: "IdOrder",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPPOO_ProfessionalDirectionOrder_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                        column: x => x.IdProfessionalDirection,
                        principalTable: "SPPOO_ProfessionalDirection",
                        principalColumn: "IdProfessionalDirection",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPPOO_ProfessionOrder",
                columns: table => new
                {
                    IdProfessionOrder = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProfession = table.Column<int>(type: "int", nullable: false),
                    IdSPPOOOrder = table.Column<int>(type: "int", nullable: false),
                    IdTypeChange = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_ProfessionOrder", x => x.IdProfessionOrder);
                    table.ForeignKey(
                        name: "FK_SPPOO_ProfessionOrder_SPPOO_Order_IdSPPOOOrder",
                        column: x => x.IdSPPOOOrder,
                        principalTable: "SPPOO_Order",
                        principalColumn: "IdOrder",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SPPOO_ProfessionOrder_SPPOO_Profession_IdProfession",
                        column: x => x.IdProfession,
                        principalTable: "SPPOO_Profession",
                        principalColumn: "IdProfession",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_ProfessionalDirectionOrder_IdProfessionalDirection",
                table: "SPPOO_ProfessionalDirectionOrder",
                column: "IdProfessionalDirection");

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_ProfessionalDirectionOrder_IdSPPOOOrder",
                table: "SPPOO_ProfessionalDirectionOrder",
                column: "IdSPPOOOrder");

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_ProfessionOrder_IdProfession",
                table: "SPPOO_ProfessionOrder",
                column: "IdProfession");

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_ProfessionOrder_IdSPPOOOrder",
                table: "SPPOO_ProfessionOrder",
                column: "IdSPPOOOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SPPOO_ProfessionalDirectionOrder");

            migrationBuilder.DropTable(
                name: "SPPOO_ProfessionOrder");
        }
    }
}
