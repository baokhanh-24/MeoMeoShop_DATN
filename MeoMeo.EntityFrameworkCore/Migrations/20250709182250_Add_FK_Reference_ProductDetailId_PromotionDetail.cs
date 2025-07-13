using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_FK_Reference_ProductDetailId_PromotionDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProductDetailId",
                table: "PromotionDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("CD9B1BD8-1326-4386-B084-2D8CFC78A6C2"));

            migrationBuilder.CreateIndex(
                name: "IX_PromotionDetails_ProductDetailId",
                table: "PromotionDetails",
                column: "ProductDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_ProductDetails_ProductDetailId",
                table: "PromotionDetails",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_ProductDetails_ProductDetailId",
                table: "PromotionDetails");

            migrationBuilder.DropIndex(
                name: "IX_PromotionDetails_ProductDetailId",
                table: "PromotionDetails");

            migrationBuilder.DropColumn(
                name: "ProductDetailId",
                table: "PromotionDetails");
        }
    }
}
