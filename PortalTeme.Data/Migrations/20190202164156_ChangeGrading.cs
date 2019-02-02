using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class ChangeGrading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Grading",
                table: "StudentAssignedTask",
                newName: "FinalGrading");

            migrationBuilder.AddColumn<int>(
                name: "Grading",
                table: "TaskSubmissions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review",
                table: "TaskSubmissions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "TaskSubmissions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grading",
                table: "TaskSubmissions");

            migrationBuilder.DropColumn(
                name: "Review",
                table: "TaskSubmissions");

            migrationBuilder.DropColumn(
                name: "State",
                table: "TaskSubmissions");

            migrationBuilder.RenameColumn(
                name: "FinalGrading",
                table: "StudentAssignedTask",
                newName: "Grading");
        }
    }
}
