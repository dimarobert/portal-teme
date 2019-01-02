using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class FixGroupsTableName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroup_Group_GroupId",
                table: "CourseGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_StudyDomain_DomainId",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_AcademicYears_YearId",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Group_GroupId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameIndex(
                name: "IX_Group_YearId",
                table: "Groups",
                newName: "IX_Groups_YearId");

            migrationBuilder.RenameIndex(
                name: "IX_Group_DomainId",
                table: "Groups",
                newName: "IX_Groups_DomainId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroup_Groups_GroupId",
                table: "CourseGroup",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_StudyDomain_DomainId",
                table: "Groups",
                column: "DomainId",
                principalTable: "StudyDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AcademicYears_YearId",
                table: "Groups",
                column: "YearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroup_Groups_GroupId",
                table: "CourseGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_StudyDomain_DomainId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AcademicYears_YearId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Groups_GroupId",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_YearId",
                table: "Group",
                newName: "IX_Group_YearId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_DomainId",
                table: "Group",
                newName: "IX_Group_DomainId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroup_Group_GroupId",
                table: "CourseGroup",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_StudyDomain_DomainId",
                table: "Group",
                column: "DomainId",
                principalTable: "StudyDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_AcademicYears_YearId",
                table: "Group",
                column: "YearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Group_GroupId",
                table: "Students",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
