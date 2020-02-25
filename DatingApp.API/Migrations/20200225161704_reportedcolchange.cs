using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.API.Migrations
{
    public partial class reportedcolchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReportedId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReportedId",
                table: "Reports",
                newName: "ReportedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReportedId",
                table: "Reports",
                newName: "IX_Reports_ReportedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReportedUserId",
                table: "Reports",
                column: "ReportedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_AspNetUsers_ReportedUserId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "ReportedUserId",
                table: "Reports",
                newName: "ReportedId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReportedUserId",
                table: "Reports",
                newName: "IX_Reports_ReportedId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_AspNetUsers_ReportedId",
                table: "Reports",
                column: "ReportedId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
