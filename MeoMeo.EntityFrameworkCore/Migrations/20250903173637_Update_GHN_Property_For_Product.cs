using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Update_GHN_Property_For_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "ProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 15);

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "ProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 15);

            migrationBuilder.AddColumn<int>(
                name: "MaxBuyPerOrder",
                table: "ProductDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "ProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 500);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "ProductDetails",
                type: "int",
                nullable: false,
                defaultValue: 15);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "MaxBuyPerOrder",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ProductDetails");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "ProductDetails");
        }
    }
}
