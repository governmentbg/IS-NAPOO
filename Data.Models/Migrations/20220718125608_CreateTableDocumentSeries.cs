using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableDocumentSeries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Request_DocumentSeries",
                columns: table => new
                {
                    IdDocumentSeries = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTypeOfRequestedDocument = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  Тип документ към печатница на МОН"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    SeriesName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_DocumentSeries", x => x.IdDocumentSeries);
                    table.ForeignKey(
                        name: "FK_Request_DocumentSeries_Request_TypeOfRequestedDocument_IdTypeOfRequestedDocument",
                        column: x => x.IdTypeOfRequestedDocument,
                        principalTable: "Request_TypeOfRequestedDocument",
                        principalColumn: "IdTypeOfRequestedDocument",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_DocumentSeries_IdTypeOfRequestedDocument",
                table: "Request_DocumentSeries",
                column: "IdTypeOfRequestedDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Request_DocumentSeries");
        }
    }
}
