using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTablesAreaProfessionalDirectionSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Profession",
                table: "Profession");

            migrationBuilder.RenameTable(
                name: "Profession",
                newName: "SPPOO_Profession");

            migrationBuilder.RenameColumn(
                name: "IdArea",
                table: "SPPOO_Profession",
                newName: "IdModifyUser");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "SPPOO_Profession",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "SPPOO_Profession",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "SPPOO_Profession",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SPPOO_Profession",
                table: "SPPOO_Profession",
                column: "IdProfession");

            migrationBuilder.CreateTable(
                name: "SPPOO_Area",
                columns: table => new
                {
                    IdArea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_Area", x => x.IdArea);
                });

            migrationBuilder.CreateTable(
                name: "SPPOO_Order",
                columns: table => new
                {
                    IdOrder = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_Order", x => x.IdOrder);
                });

            migrationBuilder.CreateTable(
                name: "SPPOO_Speciality",
                columns: table => new
                {
                    IdSpeciality = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProfession = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_Speciality", x => x.IdSpeciality);
                    table.ForeignKey(
                        name: "FK_SPPOO_Speciality_SPPOO_Profession_IdProfession",
                        column: x => x.IdProfession,
                        principalTable: "SPPOO_Profession",
                        principalColumn: "IdProfession",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SPPOO_ProfessionalDirection",
                columns: table => new
                {
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SPPOO_ProfessionalDirection", x => x.IdProfessionalDirection);
                    table.ForeignKey(
                        name: "FK_SPPOO_ProfessionalDirection_SPPOO_Area_IdArea",
                        column: x => x.IdArea,
                        principalTable: "SPPOO_Area",
                        principalColumn: "IdArea",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_Profession_IdProfessionalDirection",
                table: "SPPOO_Profession",
                column: "IdProfessionalDirection");

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_ProfessionalDirection_IdArea",
                table: "SPPOO_ProfessionalDirection",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_SPPOO_Speciality_IdProfession",
                table: "SPPOO_Speciality",
                column: "IdProfession");

            migrationBuilder.AddForeignKey(
                name: "FK_SPPOO_Profession_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "SPPOO_Profession",
                column: "IdProfessionalDirection",
                principalTable: "SPPOO_ProfessionalDirection",
                principalColumn: "IdProfessionalDirection",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SPPOO_Profession_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "SPPOO_Profession");

            migrationBuilder.DropTable(
                name: "SPPOO_Order");

            migrationBuilder.DropTable(
                name: "SPPOO_ProfessionalDirection");

            migrationBuilder.DropTable(
                name: "SPPOO_Speciality");

            migrationBuilder.DropTable(
                name: "SPPOO_Area");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SPPOO_Profession",
                table: "SPPOO_Profession");

            migrationBuilder.DropIndex(
                name: "IX_SPPOO_Profession_IdProfessionalDirection",
                table: "SPPOO_Profession");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "SPPOO_Profession");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "SPPOO_Profession");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "SPPOO_Profession");

            migrationBuilder.RenameTable(
                name: "SPPOO_Profession",
                newName: "Profession");

            migrationBuilder.RenameColumn(
                name: "IdModifyUser",
                table: "Profession",
                newName: "IdArea");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Profession",
                table: "Profession",
                column: "IdProfession");
        }
    }
}
