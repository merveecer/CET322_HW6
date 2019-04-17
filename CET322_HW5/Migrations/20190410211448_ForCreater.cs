using Microsoft.EntityFrameworkCore.Migrations;

namespace CET322_HW5.Migrations
{
    public partial class ForCreater : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SchoolUserId",
                table: "Students",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolUserId",
                table: "Students",
                column: "SchoolUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_SchoolUserId",
                table: "Students",
                column: "SchoolUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_SchoolUserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_SchoolUserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "SchoolUserId",
                table: "Students");
        }
    }
}
