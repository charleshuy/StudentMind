using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMind.Infracstructure.Migrations
{
    /// <inheritdoc />
    public partial class TempMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_SurveyTypes_TypeId",
                table: "Surveys");

            migrationBuilder.AlterColumn<string>(
                name: "TypeId",
                table: "Surveys",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_SurveyTypes_TypeId",
                table: "Surveys",
                column: "TypeId",
                principalTable: "SurveyTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Surveys_SurveyTypes_TypeId",
                table: "Surveys");

            migrationBuilder.AlterColumn<string>(
                name: "TypeId",
                table: "Surveys",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_SurveyTypes_TypeId",
                table: "Surveys",
                column: "TypeId",
                principalTable: "SurveyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
