using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Update_Table_Order_And_OrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_PromotionDetails_PromotionDetailId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderDetails");

            migrationBuilder.RenameColumn(
                name: "IventoryBatchId",
                table: "OrderDetails",
                newName: "InventoryBatchId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_IventoryBatchId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_InventoryBatchId");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "PromotionDetailId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<float>(
                name: "Discount",
                table: "OrderDetails",
                type: "real",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_InventoryBatches_InventoryBatchId",
                table: "OrderDetails",
                column: "InventoryBatchId",
                principalTable: "InventoryBatches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_PromotionDetails_PromotionDetailId",
                table: "OrderDetails",
                column: "PromotionDetailId",
                principalTable: "PromotionDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_InventoryBatches_InventoryBatchId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_PromotionDetails_PromotionDetailId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "InventoryBatchId",
                table: "OrderDetails",
                newName: "IventoryBatchId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_InventoryBatchId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_IventoryBatchId");

            migrationBuilder.AlterColumn<Guid>(
                name: "PromotionDetailId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Discount",
                table: "OrderDetails",
                type: "real",
                nullable: false,
                defaultValue: 0f,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "OrderDetails",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_InventoryBatches_IventoryBatchId",
                table: "OrderDetails",
                column: "IventoryBatchId",
                principalTable: "InventoryBatches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_PromotionDetails_PromotionDetailId",
                table: "OrderDetails",
                column: "PromotionDetailId",
                principalTable: "PromotionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
