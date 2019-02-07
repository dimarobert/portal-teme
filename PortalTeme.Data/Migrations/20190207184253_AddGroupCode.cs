using Microsoft.EntityFrameworkCore.Migrations;

namespace PortalTeme.Data.Migrations
{
    public partial class AddGroupCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Groups",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Groups");
        }
    }
}
