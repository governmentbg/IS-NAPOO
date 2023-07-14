using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTablesAssessment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assess_Survey",
                columns: table => new
                {
                    IdSurvey = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Име на анкета"),
                    AdditionalText = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false, comment: "Допълнителен текст"),
                    IdSurveyТype = table.Column<int>(type: "int", nullable: true, comment: "Тип анкета"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assess_Survey", x => x.IdSurvey);
                });

            migrationBuilder.CreateTable(
                name: "Assess_Question",
                columns: table => new
                {
                    IdQuestion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSurvey = table.Column<int>(type: "int", nullable: false, comment: "Връзка с анкета"),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Върпос"),
                    IdQuestType = table.Column<int>(type: "int", nullable: true, comment: "Тип въпрос"),
                    IdNext = table.Column<int>(type: "int", nullable: true, comment: "Следващ въпрос"),
                    IdPrev = table.Column<int>(type: "int", nullable: true, comment: "Предишен въпрос"),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, comment: "Задължителен въпрос"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assess_Question", x => x.IdQuestion);
                    table.ForeignKey(
                        name: "FK_Assess_Question_Assess_Survey_IdSurvey",
                        column: x => x.IdSurvey,
                        principalTable: "Assess_Survey",
                        principalColumn: "IdSurvey",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Въпрос към анкета");

            migrationBuilder.CreateTable(
                name: "Assess_SurveyResult",
                columns: table => new
                {
                    IdSurveyResult = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSurvey = table.Column<int>(type: "int", nullable: false, comment: "Връзка с анкета"),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Токен за валидация "),
                    TotalPointsReceived = table.Column<int>(type: "int", nullable: false, comment: "Брой точки"),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false, comment: "Резултатите са прегледани"),
                    FeedBack = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true, comment: "Коментарт към анкетата"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на започване"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Дата на приключване"),
                    IdStatus = table.Column<int>(type: "int", nullable: true, comment: "Статус"),
                    IdCandidate_Provider = table.Column<int>(type: "int", nullable: false, comment: "Връзка с  CPO,CIPO - Кандидат Обучаваща институция"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assess_SurveyResult", x => x.IdSurveyResult);
                    table.ForeignKey(
                        name: "FK_Assess_SurveyResult_Assess_Survey_IdSurvey",
                        column: x => x.IdSurvey,
                        principalTable: "Assess_Survey",
                        principalColumn: "IdSurvey",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assess_SurveyResult_Candidate_Provider_IdCandidate_Provider",
                        column: x => x.IdCandidate_Provider,
                        principalTable: "Candidate_Provider",
                        principalColumn: "IdCandidate_Provider",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assess_Answer",
                columns: table => new
                {
                    IdAnswer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdQuestion = table.Column<int>(type: "int", nullable: false, comment: "Връзка с въпрос"),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Отговор"),
                    Points = table.Column<int>(type: "int", nullable: true, comment: "Точки"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assess_Answer", x => x.IdAnswer);
                    table.ForeignKey(
                        name: "FK_Assess_Answer_Assess_Question_IdQuestion",
                        column: x => x.IdQuestion,
                        principalTable: "Assess_Question",
                        principalColumn: "IdQuestion",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Възможен отговор към върпос");

            migrationBuilder.CreateTable(
                name: "Assess_UserAnswerOpen",
                columns: table => new
                {
                    IdUserAnswerOpen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdSurveyResult = table.Column<int>(type: "int", nullable: false, comment: "Връзка с резултати анкета"),
                    IdQuestion = table.Column<int>(type: "int", nullable: false, comment: "Връзка с въпрос"),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Отговор на потребител"),
                    Points = table.Column<int>(type: "int", nullable: true, comment: "Точки"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assess_UserAnswerOpen", x => x.IdUserAnswerOpen);
                    table.ForeignKey(
                        name: "FK_Assess_UserAnswerOpen_Assess_Question_IdQuestion",
                        column: x => x.IdQuestion,
                        principalTable: "Assess_Question",
                        principalColumn: "IdQuestion",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Assess_UserAnswerOpen_Assess_SurveyResult_IdSurveyResult",
                        column: x => x.IdSurveyResult,
                        principalTable: "Assess_SurveyResult",
                        principalColumn: "IdSurveyResult",
                        onDelete: ReferentialAction.NoAction);
                },
                comment: "Отговор на въпрос");

            migrationBuilder.CreateTable(
                name: "Assess_UserAnswer",
                columns: table => new
                {
                    IdUserAnswer = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUserAnswerOpen = table.Column<int>(type: "int", nullable: false, comment: "Връзка с отворен отговор"),
                    IdQuestion = table.Column<int>(type: "int", nullable: false, comment: "Връзка с въпрос"),
                    Points = table.Column<int>(type: "int", nullable: true, comment: "Точки")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assess_UserAnswer", x => x.IdUserAnswer);
                    table.ForeignKey(
                        name: "FK_Assess_UserAnswer_Assess_Question_IdQuestion",
                        column: x => x.IdQuestion,
                        principalTable: "Assess_Question",
                        principalColumn: "IdQuestion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assess_UserAnswer_Assess_UserAnswerOpen_IdUserAnswerOpen",
                        column: x => x.IdUserAnswerOpen,
                        principalTable: "Assess_UserAnswerOpen",
                        principalColumn: "IdUserAnswerOpen",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Отговор на въпрос");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_Answer_IdQuestion",
                table: "Assess_Answer",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_Question_IdSurvey",
                table: "Assess_Question",
                column: "IdSurvey");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_SurveyResult_IdCandidate_Provider",
                table: "Assess_SurveyResult",
                column: "IdCandidate_Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_SurveyResult_IdSurvey",
                table: "Assess_SurveyResult",
                column: "IdSurvey");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_UserAnswer_IdQuestion",
                table: "Assess_UserAnswer",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_UserAnswer_IdUserAnswerOpen",
                table: "Assess_UserAnswer",
                column: "IdUserAnswerOpen");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_UserAnswerOpen_IdQuestion",
                table: "Assess_UserAnswerOpen",
                column: "IdQuestion");

            migrationBuilder.CreateIndex(
                name: "IX_Assess_UserAnswerOpen_IdSurveyResult",
                table: "Assess_UserAnswerOpen",
                column: "IdSurveyResult");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assess_Answer");

            migrationBuilder.DropTable(
                name: "Assess_UserAnswer");

            migrationBuilder.DropTable(
                name: "Assess_UserAnswerOpen");

            migrationBuilder.DropTable(
                name: "Assess_Question");

            migrationBuilder.DropTable(
                name: "Assess_SurveyResult");

            migrationBuilder.DropTable(
                name: "Assess_Survey");
        }
    }
}
