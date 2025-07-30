using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShoeLength",
                table: "ProductDetails");

            migrationBuilder.AddColumn<int>(
                name: "ClosureType",
                table: "ProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosureType",
                table: "ProductDetails");

            migrationBuilder.AddColumn<float>(
                name: "ShoeLength",
                table: "ProductDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
