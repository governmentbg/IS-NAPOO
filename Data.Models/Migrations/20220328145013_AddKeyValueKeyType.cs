using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddKeyValueKeyType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.CreateTable(
                name: "KeyType",
                columns: table => new
                {
                    IdKeyType = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    KeyTypeIntCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyType", x => x.IdKeyType);
                });

            migrationBuilder.CreateTable(
                name: "Profession",
                columns: table => new
                {
                    IdProfession = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdArea = table.Column<int>(type: "int", nullable: false),
                    IdProfessionalDirection = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profession", x => x.IdProfession);
                });

            migrationBuilder.CreateTable(
                name: "KeyValue",
                columns: table => new
                {
                    IdKeyValue = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdKeyType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    KeyValueIntCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    DescriptionEN = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    DefaultValue1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DefaultValue2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FormattedText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    FormattedTextEN = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyValue", x => x.IdKeyValue);
                    table.ForeignKey(
                        name: "FK_KeyValue_KeyType_IdKeyType",
                        column: x => x.IdKeyType,
                        principalTable: "KeyType",
                        principalColumn: "IdKeyType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeyValue_IdKeyType",
                table: "KeyValue",
                column: "IdKeyType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeyValue");

            migrationBuilder.DropTable(
                name: "Profession");

            migrationBuilder.DropTable(
                name: "KeyType");

            migrationBuilder.CreateTable(
                name: "CodeCorrection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrectionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeCorrection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodeVetGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCodeCorrection = table.Column<int>(type: "int", nullable: false),
                    GroupNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodeVetGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodeVetGroup_CodeCorrection_IdCodeCorrection",
                        column: x => x.IdCodeCorrection,
                        principalTable: "CodeCorrection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CodeVetGroup_IdCodeCorrection",
                table: "CodeVetGroup",
                column: "IdCodeCorrection");
        }
    }
}
