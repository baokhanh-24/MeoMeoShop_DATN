using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_SizeId_For_Cart_InventoryBatch_Promotion_OrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Promotions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Promotions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "PromotionDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "OriginalPrice",
                table: "OrderDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("C17BCB4A-8F96-48B6-BC5B-13C99D479C69"));

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "InventoryBatches",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("C17BCB4A-8F96-48B6-BC5B-13C99D479C69"));

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "CartDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("C17BCB4A-8F96-48B6-BC5B-13C99D479C69"));

            migrationBuilder.CreateIndex(
                name: "IX_PromotionDetails_SizeId",
                table: "PromotionDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_SizeId",
                table: "OrderDetails",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatches_SizeId",
                table: "InventoryBatches",
                column: "SizeId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_SizeId",
                table: "CartDetails",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_Sizes_SizeId",
                table: "CartDetails",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBatches_Sizes_SizeId",
                table: "InventoryBatches",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Sizes_SizeId",
                table: "OrderDetails",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_Sizes_SizeId",
                table: "PromotionDetails",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_Sizes_SizeId",
                table: "CartDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBatches_Sizes_SizeId",
                table: "InventoryBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Sizes_SizeId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_Sizes_SizeId",
                table: "PromotionDetails");

            migrationBuilder.DropIndex(
                name: "IX_PromotionDetails_SizeId",
                table: "PromotionDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_SizeId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_InventoryBatches_SizeId",
                table: "InventoryBatches");

            migrationBuilder.DropIndex(
                name: "IX_CartDetails_SizeId",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "PromotionDetails");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "InventoryBatches");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "CartDetails");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Promotions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Promotions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
