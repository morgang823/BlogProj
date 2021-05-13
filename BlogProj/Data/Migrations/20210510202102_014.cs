using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogProj.Data.Migrations
{
    public partial class _014 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imageData",
                table: "AspNetUsers",
                newName: "ImageData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageData",
                table: "AspNetUsers",
                newName: "imageData");
        }
    }
}
