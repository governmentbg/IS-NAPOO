using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientCourseAllowNULL_IdProfessionalDirection_IdSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_ClientCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_Speciality_IdSpeciality",
                table: "Training_ClientCourse");

            migrationBuilder.AlterColumn<int>(
                name: "IdSpeciality",
                table: "Training_ClientCourse",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdProfessionalDirection",
                table: "Training_ClientCourse",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_ClientCourse",
                column: "IdProfessionalDirection",
                principalTable: "SPPOO_ProfessionalDirection",
                principalColumn: "IdProfessionalDirection");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_Speciality_IdSpeciality",
                table: "Training_ClientCourse",
                column: "IdSpeciality",
                principalTable: "SPPOO_Speciality",
                principalColumn: "IdSpeciality");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_ClientCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_Speciality_IdSpeciality",
                table: "Training_ClientCourse");

            migrationBuilder.AlterColumn<int>(
                name: "IdSpeciality",
                table: "Training_ClientCourse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdProfessionalDirection",
                table: "Training_ClientCourse",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_ProfessionalDirection_IdProfessionalDirection",
                table: "Training_ClientCourse",
                column: "IdProfessionalDirection",
                principalTable: "SPPOO_ProfessionalDirection",
                principalColumn: "IdProfessionalDirection",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourse_SPPOO_Speciality_IdSpeciality",
                table: "Training_ClientCourse",
                column: "IdSpeciality",
                principalTable: "SPPOO_Speciality",
                principalColumn: "IdSpeciality",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
