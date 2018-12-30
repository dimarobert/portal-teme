using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class FixTableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseDefinition_AcademicYear_YearId",
                table: "CourseDefinition");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseDefinition_CourseInfoId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_AcademicYear_YearId",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseDefinition",
                table: "CourseDefinition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicYear",
                table: "AcademicYear");

            migrationBuilder.RenameTable(
                name: "CourseDefinition",
                newName: "CourseDefinitions");

            migrationBuilder.RenameTable(
                name: "AcademicYear",
                newName: "AcademicYears");

            migrationBuilder.RenameIndex(
                name: "IX_CourseDefinition_YearId",
                table: "CourseDefinitions",
                newName: "IX_CourseDefinitions_YearId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseDefinitions",
                table: "CourseDefinitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicYears",
                table: "AcademicYears",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDefinitions_AcademicYears_YearId",
                table: "CourseDefinitions",
                column: "YearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseDefinitions_CourseInfoId",
                table: "Courses",
                column: "CourseInfoId",
                principalTable: "CourseDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_AcademicYears_YearId",
                table: "Group",
                column: "YearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseDefinitions_AcademicYears_YearId",
                table: "CourseDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseDefinitions_CourseInfoId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_AcademicYears_YearId",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseDefinitions",
                table: "CourseDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicYears",
                table: "AcademicYears");

            migrationBuilder.RenameTable(
                name: "CourseDefinitions",
                newName: "CourseDefinition");

            migrationBuilder.RenameTable(
                name: "AcademicYears",
                newName: "AcademicYear");

            migrationBuilder.RenameIndex(
                name: "IX_CourseDefinitions_YearId",
                table: "CourseDefinition",
                newName: "IX_CourseDefinition_YearId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseDefinition",
                table: "CourseDefinition",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicYear",
                table: "AcademicYear",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDefinition_AcademicYear_YearId",
                table: "CourseDefinition",
                column: "YearId",
                principalTable: "AcademicYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseDefinition_CourseInfoId",
                table: "Courses",
                column: "CourseInfoId",
                principalTable: "CourseDefinition",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_AcademicYear_YearId",
                table: "Group",
                column: "YearId",
                principalTable: "AcademicYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
