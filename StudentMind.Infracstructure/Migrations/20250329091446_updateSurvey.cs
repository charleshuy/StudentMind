using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMind.Infracstructure.Migrations
{
    /// <inheritdoc />
    public partial class updateSurvey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Surveys_SurveyId",
                table: "SurveyResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Users_UserId",
                table: "SurveyResponses");

            migrationBuilder.AddColumn<string>(
                name: "ChoiceId",
                table: "SurveyResponses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuestionId",
                table: "SurveyResponses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HealthScoreRules",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SurveyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MinScore = table.Column<int>(type: "int", nullable: false),
                    MaxScore = table.Column<int>(type: "int", nullable: false),
                    category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastUpdatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthScoreRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HealthScoreRules_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponses_ChoiceId",
                table: "SurveyResponses",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResponses_QuestionId",
                table: "SurveyResponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_HealthScoreRules_SurveyId",
                table: "HealthScoreRules",
                column: "SurveyId");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Choices_ChoiceId",
                table: "SurveyResponses",
                column: "ChoiceId",
                principalTable: "Choices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Questions_QuestionId",
                table: "SurveyResponses",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Surveys_SurveyId",
                table: "SurveyResponses",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Users_UserId",
                table: "SurveyResponses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Choices_ChoiceId",
                table: "SurveyResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Questions_QuestionId",
                table: "SurveyResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Surveys_SurveyId",
                table: "SurveyResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyResponses_Users_UserId",
                table: "SurveyResponses");

            migrationBuilder.DropTable(
                name: "HealthScoreRules");

            migrationBuilder.DropIndex(
                name: "IX_SurveyResponses_ChoiceId",
                table: "SurveyResponses");

            migrationBuilder.DropIndex(
                name: "IX_SurveyResponses_QuestionId",
                table: "SurveyResponses");

            migrationBuilder.DropColumn(
                name: "ChoiceId",
                table: "SurveyResponses");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "SurveyResponses");

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Surveys_SurveyId",
                table: "SurveyResponses",
                column: "SurveyId",
                principalTable: "Surveys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyResponses_Users_UserId",
                table: "SurveyResponses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
