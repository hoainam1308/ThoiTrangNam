using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThoiTrangNam.Migrations
{
    /// <inheritdoc />
    public partial class updateProductLocDau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemovedDiacriticsName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemovedDiacriticsName",
                table: "Products");
        }
    }
}
