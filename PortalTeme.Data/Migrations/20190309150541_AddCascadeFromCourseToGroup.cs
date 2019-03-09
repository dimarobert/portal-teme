using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class AddCascadeFromCourseToGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseDefinitions_AcademicYears_YearId",
                table: "CourseDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroup_Courses_CourseId",
                table: "CourseGroup");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDefinitions_AcademicYears_YearId",
                table: "CourseDefinitions",
                column: "YearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroup_Courses_CourseId",
                table: "CourseGroup",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseDefinitions_AcademicYears_YearId",
                table: "CourseDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroup_Courses_CourseId",
                table: "CourseGroup");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDefinitions_AcademicYears_YearId",
                table: "CourseDefinitions",
                column: "YearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroup_Courses_CourseId",
                table: "CourseGroup",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
