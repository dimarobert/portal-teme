using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class UpdateTaskSubmissionFilesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "TaskSubmissionFile");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "TaskSubmissionFile",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TaskSubmissionFile_FileId",
                table: "TaskSubmissionFile",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSubmissionFile_Files_FileId",
                table: "TaskSubmissionFile",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSubmissionFile_Files_FileId",
                table: "TaskSubmissionFile");

            migrationBuilder.DropIndex(
                name: "IX_TaskSubmissionFile_FileId",
                table: "TaskSubmissionFile");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "TaskSubmissionFile");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TaskSubmissionFile",
                nullable: false,
                defaultValue: "");
        }
    }
}
