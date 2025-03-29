using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMind.Infracstructure.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
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
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Appointments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_SurveyTypes_TypeId",
                table: "Surveys",
                column: "TypeId",
                principalTable: "SurveyTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
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
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Surveys_SurveyTypes_TypeId",
                table: "Surveys",
                column: "TypeId",
                principalTable: "SurveyTypes",
                principalColumn: "Id");
        }
    }
}
