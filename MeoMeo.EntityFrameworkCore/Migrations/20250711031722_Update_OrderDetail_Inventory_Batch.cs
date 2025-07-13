using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Update_OrderDetail_Inventory_Batch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_InventoryBatches_InventoryBatchId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_ProductDetails_ProductDetailId",
                table: "PromotionDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_InventoryBatchId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "InventoryBatchId",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "OrderDetailInventoryBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryBatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailInventoryBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetailInventoryBatches_InventoryBatches_InventoryBatchId",
                        column: x => x.InventoryBatchId,
                        principalTable: "InventoryBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderDetailInventoryBatches_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailInventoryBatches_InventoryBatchId",
                table: "OrderDetailInventoryBatches",
                column: "InventoryBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailInventoryBatches_OrderDetailId",
                table: "OrderDetailInventoryBatches",
                column: "OrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_ProductDetails_ProductDetailId",
                table: "PromotionDetails",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionDetails_ProductDetails_ProductDetailId",
                table: "PromotionDetails");

            migrationBuilder.DropTable(
                name: "OrderDetailInventoryBatches");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryBatchId",
                table: "OrderDetails",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_InventoryBatchId",
                table: "OrderDetails",
                column: "InventoryBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_InventoryBatches_InventoryBatchId",
                table: "OrderDetails",
                column: "InventoryBatchId",
                principalTable: "InventoryBatches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionDetails_ProductDetails_ProductDetailId",
                table: "PromotionDetails",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
