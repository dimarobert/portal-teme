using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class RestructureAssignments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentExtensionRequests_AssignmentEntries_AssignmentEntryId",
                table: "AssignmentExtensionRequests");

            migrationBuilder.DropTable(
                name: "AssignmentEntryFile");

            migrationBuilder.DropTable(
                name: "AssignmentVariant");

            migrationBuilder.DropTable(
                name: "AssignmentEntryVersion");

            migrationBuilder.DropTable(
                name: "AssignmentEntries");

            migrationBuilder.RenameColumn(
                name: "AssignmentEntryId",
                table: "AssignmentExtensionRequests",
                newName: "TaskSubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentExtensionRequests_AssignmentEntryId",
                table: "AssignmentExtensionRequests",
                newName: "IX_AssignmentExtensionRequests_TaskSubmissionId");

            migrationBuilder.CreateTable(
                name: "AssignmentTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentTask_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAssignedTask",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(nullable: false),
                    StudentId = table.Column<string>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Grading = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAssignedTask", x => new { x.TaskId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_StudentAssignedTask_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentAssignedTask_AssignmentTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "AssignmentTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TaskSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false),
                    TaskStudentId = table.Column<string>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSubmissions_StudentAssignedTask_TaskId_TaskStudentId",
                        columns: x => new { x.TaskId, x.TaskStudentId },
                        principalTable: "StudentAssignedTask",
                        principalColumns: new[] { "TaskId", "StudentId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskSubmissionFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FileType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TaskSubmissionId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSubmissionFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskSubmissionFile_TaskSubmissions_TaskSubmissionId",
                        column: x => x.TaskSubmissionId,
                        principalTable: "TaskSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentTask_AssignmentId",
                table: "AssignmentTask",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignedTask_StudentId",
                table: "StudentAssignedTask",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmissionFile_TaskSubmissionId",
                table: "TaskSubmissionFile",
                column: "TaskSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmissions_TaskId_TaskStudentId",
                table: "TaskSubmissions",
                columns: new[] { "TaskId", "TaskStudentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentExtensionRequests_TaskSubmissions_TaskSubmissionId",
                table: "AssignmentExtensionRequests",
                column: "TaskSubmissionId",
                principalTable: "TaskSubmissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentExtensionRequests_TaskSubmissions_TaskSubmissionId",
                table: "AssignmentExtensionRequests");

            migrationBuilder.DropTable(
                name: "TaskSubmissionFile");

            migrationBuilder.DropTable(
                name: "TaskSubmissions");

            migrationBuilder.DropTable(
                name: "StudentAssignedTask");

            migrationBuilder.DropTable(
                name: "AssignmentTask");

            migrationBuilder.RenameColumn(
                name: "TaskSubmissionId",
                table: "AssignmentExtensionRequests",
                newName: "AssignmentEntryId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentExtensionRequests_TaskSubmissionId",
                table: "AssignmentExtensionRequests",
                newName: "IX_AssignmentExtensionRequests_AssignmentEntryId");

            migrationBuilder.CreateTable(
                name: "AssignmentEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentId = table.Column<Guid>(nullable: false),
                    Grading = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    StudentUserId = table.Column<string>(nullable: false)
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
                        name: "FK_AssignmentEntries_Students_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "Students",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentVariant",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    StudentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssignmentVariant_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentVariant_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "AssignmentEntryFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentVersionId = table.Column<Guid>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FileType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntries_AssignmentId",
                table: "AssignmentEntries",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntries_StudentUserId",
                table: "AssignmentEntries",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntryFile_AssignmentVersionId",
                table: "AssignmentEntryFile",
                column: "AssignmentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentEntryVersion_AssignmentEntryId",
                table: "AssignmentEntryVersion",
                column: "AssignmentEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentVariant_AssignmentId",
                table: "AssignmentVariant",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentVariant_StudentId",
                table: "AssignmentVariant",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentExtensionRequests_AssignmentEntries_AssignmentEntryId",
                table: "AssignmentExtensionRequests",
                column: "AssignmentEntryId",
                principalTable: "AssignmentEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
