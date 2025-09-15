using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_Import_Batches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBatches_ProductDetails_ProductDetailId",
                table: "InventoryBatches");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "InventoryBatches");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "InventoryBatches");

            migrationBuilder.AddColumn<Guid>(
                name: "ImportBatchId",
                table: "InventoryBatches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImportBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    ImportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportBatches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatches_ImportBatchId",
                table: "InventoryBatches",
                column: "ImportBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportBatches_Code",
                table: "ImportBatches",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBatches_ImportBatches_ImportBatchId",
                table: "InventoryBatches",
                column: "ImportBatchId",
                principalTable: "ImportBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBatches_ProductDetails_ProductDetailId",
                table: "InventoryBatches",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBatches_ImportBatches_ImportBatchId",
                table: "InventoryBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBatches_ProductDetails_ProductDetailId",
                table: "InventoryBatches");

            migrationBuilder.DropTable(
                name: "ImportBatches");

            migrationBuilder.DropIndex(
                name: "IX_InventoryBatches_ImportBatchId",
                table: "InventoryBatches");

            migrationBuilder.DropColumn(
                name: "ImportBatchId",
                table: "InventoryBatches");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "InventoryBatches",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "InventoryBatches",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBatches_ProductDetails_ProductDetailId",
                table: "InventoryBatches",
                column: "ProductDetailId",
                principalTable: "ProductDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
