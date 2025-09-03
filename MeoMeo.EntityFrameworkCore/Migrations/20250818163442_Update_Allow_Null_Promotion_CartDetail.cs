using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Update_Allow_Null_Promotion_CartDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_PromotionDetails_PromotionDetailId",
                table: "CartDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "PromotionDetailId",
                table: "CartDetails",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_PromotionDetails_PromotionDetailId",
                table: "CartDetails",
                column: "PromotionDetailId",
                principalTable: "PromotionDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_PromotionDetails_PromotionDetailId",
                table: "CartDetails");

            migrationBuilder.AlterColumn<Guid>(
                name: "PromotionDetailId",
                table: "CartDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_PromotionDetails_PromotionDetailId",
                table: "CartDetails",
                column: "PromotionDetailId",
                principalTable: "PromotionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
