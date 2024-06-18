using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThoiTrangNam.Migrations
{
    /// <inheritdoc />
    public partial class addDateRateRe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "OrderDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review",
                table: "OrderDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewDate",
                table: "OrderDetails",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Review",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "ReviewDate",
                table: "OrderDetails");
        }
    }
}
