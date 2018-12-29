using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class AddStudentInfoAndFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentEntries_AspNetUsers_StudentId",
                table: "AssignmentEntries");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "AssignmentEntries",
                newName: "StudentUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentEntries_StudentId",
                table: "AssignmentEntries",
                newName: "IX_AssignmentEntries_StudentUserId");

            migrationBuilder.CreateTable(
                name: "AssignmentEntryVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentEntryId = table.Column<Guid>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentEntryVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentEntryVersion_AssignmentEntries_AssignmentEntryId",
                        column: x => x.AssignmentEntryId,
                        principalTable: "AssignmentEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyDomain",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyDomain", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentEntryFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FileType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    AssignmentVersionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentEntryFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentEntryFile_AssignmentEntryVersion_AssignmentVersionId",
                        column: x => x.AssignmentVersionId,
                        principalTable: "AssignmentEntryVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    DomainId = table.Column<Guid>(nullable: false),
                    YearId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Group_StudyDomain_DomainId",
                        column: x => x.DomainId,
                        principalTable: "StudyDomain",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Group_AcademicYear_YearId",
                        column: x => x.YearId,
                        principalTable: "AcademicYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: false),
                    Semester = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Students_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Students_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntryFile_AssignmentVersionId",
                table: "AssignmentEntryFile",
                column: "AssignmentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntryVersion_AssignmentEntryId",
                table: "AssignmentEntryVersion",
                column: "AssignmentEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_DomainId",
                table: "Group",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Group_YearId",
                table: "Group",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GroupId",
                table: "Students",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentEntries_Students_StudentUserId",
                table: "AssignmentEntries",
                column: "StudentUserId",
                principalTable: "Students",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentEntries_Students_StudentUserId",
                table: "AssignmentEntries");

            migrationBuilder.DropTable(
                name: "AssignmentEntryFile");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "AssignmentEntryVersion");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.DropTable(
                name: "StudyDomain");

            migrationBuilder.RenameColumn(
                name: "StudentUserId",
                table: "AssignmentEntries",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentEntries_StudentUserId",
                table: "AssignmentEntries",
                newName: "IX_AssignmentEntries_StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentEntries_AspNetUsers_StudentId",
                table: "AssignmentEntries",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
