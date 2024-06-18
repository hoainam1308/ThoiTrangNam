using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThoiTrangNam.Migrations
{
    /// <inheritdoc />
    public partial class updateproductavg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "AvgRating",
                table: "Products",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityRating",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgRating",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "QuantityRating",
                table: "Products");
        }
    }
}
