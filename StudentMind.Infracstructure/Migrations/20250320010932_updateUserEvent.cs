using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentMind.Infracstructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUserEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEvents_Events_ProgramId",
                table: "UserEvents");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvents_Events_ProgramId",
                table: "UserEvents",
                column: "ProgramId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEvents_Events_ProgramId",
                table: "UserEvents");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEvents_Events_ProgramId",
                table: "UserEvents",
                column: "ProgramId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
