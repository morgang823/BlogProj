using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogProj.Migrations
{
    public partial class _005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModertationType",
                table: "Comments",
                newName: "ModerationType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModerationType",
                table: "Comments",
                newName: "ModertationType");
        }
    }
}
