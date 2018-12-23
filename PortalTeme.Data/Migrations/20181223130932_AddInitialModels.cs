using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class AddInitialModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicYear",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYear", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseDefinition",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    YearId = table.Column<Guid>(nullable: false),
                    Semester = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseDefinition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseDefinition_AcademicYear_YearId",
                        column: x => x.YearId,
                        principalTable: "AcademicYear",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseInfoId = table.Column<Guid>(nullable: false),
                    ProfessorId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_CourseDefinition_CourseInfoId",
                        column: x => x.CourseInfoId,
                        principalTable: "CourseDefinition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Courses_AspNetUsers_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CourseId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseAssistant",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(nullable: false),
                    AssistantId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAssistant", x => new { x.CourseId, x.AssistantId });
                    table.ForeignKey(
                        name: "FK_CourseAssistant_AspNetUsers_AssistantId",
                        column: x => x.AssistantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseAssistant_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentId = table.Column<Guid>(nullable: false),
                    StudentId = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Grading = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentEntries_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentEntries_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentExtensionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentEntryId = table.Column<Guid>(nullable: false),
                    Reason = table.Column<string>(nullable: false),
                    Approved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentExtensionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentExtensionRequests_AssignmentEntries_AssignmentEntryId",
                        column: x => x.AssignmentEntryId,
                        principalTable: "AssignmentEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntries_AssignmentId",
                table: "AssignmentEntries",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntries_StudentId",
                table: "AssignmentEntries",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentExtensionRequests_AssignmentEntryId",
                table: "AssignmentExtensionRequests",
                column: "AssignmentEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseId",
                table: "Assignments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAssistant_AssistantId",
                table: "CourseAssistant",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseDefinition_YearId",
                table: "CourseDefinition",
                column: "YearId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseInfoId",
                table: "Courses",
                column: "CourseInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_ProfessorId",
                table: "Courses",
                column: "ProfessorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentExtensionRequests");

            migrationBuilder.DropTable(
                name: "CourseAssistant");

            migrationBuilder.DropTable(
                name: "AssignmentEntries");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "CourseDefinition");

            migrationBuilder.DropTable(
                name: "AcademicYear");
        }
    }
}
