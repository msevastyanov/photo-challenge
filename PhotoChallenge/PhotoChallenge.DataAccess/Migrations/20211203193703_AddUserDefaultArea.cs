using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhotoChallenge.DataAccess.Migrations
{
    public partial class AddUserDefaultArea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultAreaId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultAreaId",
                table: "AspNetUsers");
        }
    }
}
