using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddEKATTETables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    idDistrict = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DistrictCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    int_obl_id_old = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_District", x => x.idDistrict);
                });

            migrationBuilder.CreateTable(
                name: "Municipality",
                columns: table => new
                {
                    idMunicipality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idDistrict = table.Column<int>(type: "int", nullable: false),
                    MunicipalityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MunicipalityCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    int_municipality_id_old = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipality", x => x.idMunicipality);
                    table.ForeignKey(
                        name: "FK_Municipality_District_idDistrict",
                        column: x => x.idDistrict,
                        principalTable: "District",
                        principalColumn: "idDistrict",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    idLocation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idMunicipality = table.Column<int>(type: "int", nullable: false),
                    kati = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LocationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocationCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Cat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Height = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PostCode = table.Column<int>(type: "int", nullable: false),
                    PhoneCode = table.Column<int>(type: "int", nullable: false),
                    int_ekatte_id_old = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.idLocation);
                    table.ForeignKey(
                        name: "FK_Location_Municipality_idMunicipality",
                        column: x => x.idMunicipality,
                        principalTable: "Municipality",
                        principalColumn: "idMunicipality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    idRegion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RegionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    idMunicipality = table.Column<int>(type: "int", nullable: false),
                    int_municipality_details_id_old = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.idRegion);
                    table.ForeignKey(
                        name: "FK_Region_Municipality_idMunicipality",
                        column: x => x.idMunicipality,
                        principalTable: "Municipality",
                        principalColumn: "idMunicipality",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Location_idMunicipality",
                table: "Location",
                column: "idMunicipality");

            migrationBuilder.CreateIndex(
                name: "IX_Municipality_idDistrict",
                table: "Municipality",
                column: "idDistrict");

            migrationBuilder.CreateIndex(
                name: "IX_Region_idMunicipality",
                table: "Region",
                column: "idMunicipality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "Municipality");

            migrationBuilder.DropTable(
                name: "District");
        }
    }
}
