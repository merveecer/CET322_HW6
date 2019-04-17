using Microsoft.EntityFrameworkCore.Migrations;

namespace CET322_HW5.Migrations
{
    public partial class duzeltme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DepartmentAdminId",
                table: "Departments",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DepartmentAdminId",
                table: "Departments",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
