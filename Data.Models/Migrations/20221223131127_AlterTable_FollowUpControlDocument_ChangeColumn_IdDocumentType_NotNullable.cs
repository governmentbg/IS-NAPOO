using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_FollowUpControlDocument_ChangeColumn_IdDocumentType_NotNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdDocumentType",
                table: "Control_FollowUpControlDocument",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Тип на документа при последващ контрол",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Тип на документа при последващ контрол");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "IdDocumentType",
                table: "Control_FollowUpControlDocument",
                type: "int",
                nullable: true,
                comment: "Тип на документа при последващ контрол",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Тип на документа при последващ контрол");
        }
    }
}
