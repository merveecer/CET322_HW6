using Microsoft.EntityFrameworkCore.Migrations;

namespace CET322_HW5.Migrations
{
    public partial class Manager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentAdminId",
                table: "Departments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentAdminId",
                table: "Departments");
        }
    }
}
