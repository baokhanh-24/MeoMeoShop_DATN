using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Update_Table_Wishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_Products_ProductId",
                table: "Wishlist");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wishlist",
                table: "Wishlist");

            migrationBuilder.DropIndex(
                name: "IX_Wishlist_CustomerId_ProductId",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Wishlist");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Wishlist");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Wishlist",
                newName: "ProductDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlist_ProductId",
                table: "Wishlist",
                newName: "IX_Wishlist_ProductDetailId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wishlist",
                table: "Wishlist",
                columns: new[] { "CustomerId", "ProductDetailId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_ProductDetails_ProductDetailId",
                table: "Wishlist",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wishlist_ProductDetails_ProductDetailId",
                table: "Wishlist");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wishlist",
                table: "Wishlist");

            migrationBuilder.RenameColumn(
                name: "ProductDetailId",
                table: "Wishlist",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_Wishlist_ProductDetailId",
                table: "Wishlist",
                newName: "IX_Wishlist_ProductId");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Wishlist",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Wishlist",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Wishlist",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Wishlist",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wishlist",
                table: "Wishlist",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_CustomerId_ProductId",
                table: "Wishlist",
                columns: new[] { "CustomerId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Wishlist_Products_ProductId",
                table: "Wishlist",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
