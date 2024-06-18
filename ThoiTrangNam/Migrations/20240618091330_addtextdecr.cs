using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThoiTrangNam.Migrations
{
    /// <inheritdoc />
    public partial class addtextdecr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextDescription",
                table: "Products");
        }
    }
}
