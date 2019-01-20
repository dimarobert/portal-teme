using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class AddPkToStudentAssignTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSubmissions_StudentAssignedTask_TaskId_TaskStudentId",
                table: "TaskSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_TaskSubmissions_TaskId_TaskStudentId",
                table: "TaskSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAssignedTask",
                table: "StudentAssignedTask");

            migrationBuilder.DropColumn(
                name: "TaskStudentId",
                table: "TaskSubmissions");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "TaskSubmissions",
                newName: "AssignedTaskId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "StudentAssignedTask",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAssignedTask",
                table: "StudentAssignedTask",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmissions_AssignedTaskId",
                table: "TaskSubmissions",
                column: "AssignedTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssignedTask_TaskId",
                table: "StudentAssignedTask",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSubmissions_StudentAssignedTask_AssignedTaskId",
                table: "TaskSubmissions",
                column: "AssignedTaskId",
                principalTable: "StudentAssignedTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSubmissions_StudentAssignedTask_AssignedTaskId",
                table: "TaskSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_TaskSubmissions_AssignedTaskId",
                table: "TaskSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAssignedTask",
                table: "StudentAssignedTask");

            migrationBuilder.DropIndex(
                name: "IX_StudentAssignedTask_TaskId",
                table: "StudentAssignedTask");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudentAssignedTask");

            migrationBuilder.RenameColumn(
                name: "AssignedTaskId",
                table: "TaskSubmissions",
                newName: "TaskId");

            migrationBuilder.AddColumn<string>(
                name: "TaskStudentId",
                table: "TaskSubmissions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAssignedTask",
                table: "StudentAssignedTask",
                columns: new[] { "TaskId", "StudentId" });

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmissions_TaskId_TaskStudentId",
                table: "TaskSubmissions",
                columns: new[] { "TaskId", "TaskStudentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSubmissions_StudentAssignedTask_TaskId_TaskStudentId",
                table: "TaskSubmissions",
                columns: new[] { "TaskId", "TaskStudentId" },
                principalTable: "StudentAssignedTask",
                principalColumns: new[] { "TaskId", "StudentId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
