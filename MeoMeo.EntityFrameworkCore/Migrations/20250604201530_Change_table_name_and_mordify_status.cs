using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Change_table_name_and_mordify_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_customers_CustomerId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_Users_UserId",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomersBanks_customers_CustomerId",
                table: "CustomersBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_deliveryAddresses_Communes_CommuneId",
                table: "deliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_deliveryAddresses_Districts_DistrictId",
                table: "deliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_deliveryAddresses_Provinces_ProvinceId",
                table: "deliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_deliveryAddresses_customers_CustomerId",
                table: "deliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_PromotionDetails_ProductDetailId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_deliveryAddresses_DeliveryAddressId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_deliveryAddresses",
                table: "deliveryAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_customers",
                table: "customers");

            migrationBuilder.RenameTable(
                name: "deliveryAddresses",
                newName: "DeliveryAddresses");

            migrationBuilder.RenameTable(
                name: "customers",
                newName: "Customers");

            migrationBuilder.RenameIndex(
                name: "IX_deliveryAddresses_ProvinceId",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_deliveryAddresses_DistrictId",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_deliveryAddresses_CustomerId",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_deliveryAddresses_CommuneId",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_CommuneId");

            migrationBuilder.RenameIndex(
                name: "IX_customers_UserId",
                table: "Customers",
                newName: "IX_Customers_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Sizes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Seasons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Materials",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CustomersBanks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Colours",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CartDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryAddresses",
                table: "DeliveryAddresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_PromotionDetailId",
                table: "OrderDetails",
                column: "PromotionDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Customers_CustomerId",
                table: "Carts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersBanks_Customers_CustomerId",
                table: "CustomersBanks",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddresses_Communes_CommuneId",
                table: "DeliveryAddresses",
                column: "CommuneId",
                principalTable: "Communes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddresses_Customers_CustomerId",
                table: "DeliveryAddresses",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddresses_Districts_DistrictId",
                table: "DeliveryAddresses",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddresses_Provinces_ProvinceId",
                table: "DeliveryAddresses",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_PromotionDetails_PromotionDetailId",
                table: "OrderDetails",
                column: "PromotionDetailId",
                principalTable: "PromotionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryAddresses_DeliveryAddressId",
                table: "Orders",
                column: "DeliveryAddressId",
                principalTable: "DeliveryAddresses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Customers_CustomerId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Users_UserId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomersBanks_Customers_CustomerId",
                table: "CustomersBanks");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddresses_Communes_CommuneId",
                table: "DeliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddresses_Customers_CustomerId",
                table: "DeliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddresses_Districts_DistrictId",
                table: "DeliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddresses_Provinces_ProvinceId",
                table: "DeliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_PromotionDetails_PromotionDetailId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryAddresses_DeliveryAddressId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_PromotionDetailId",
                table: "OrderDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryAddresses",
                table: "DeliveryAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "DeliveryAddresses",
                newName: "deliveryAddresses");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "customers");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_ProvinceId",
                table: "deliveryAddresses",
                newName: "IX_deliveryAddresses_ProvinceId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_DistrictId",
                table: "deliveryAddresses",
                newName: "IX_deliveryAddresses_DistrictId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_CustomerId",
                table: "deliveryAddresses",
                newName: "IX_deliveryAddresses_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_CommuneId",
                table: "deliveryAddresses",
                newName: "IX_deliveryAddresses_CommuneId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_UserId",
                table: "customers",
                newName: "IX_customers_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Sizes",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Seasons",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProductDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Materials",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Images",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Employees",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CustomersBanks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Colours",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CartDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_deliveryAddresses",
                table: "deliveryAddresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_customers",
                table: "customers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_customers_CustomerId",
                table: "Carts",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_Users_UserId",
                table: "customers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomersBanks_customers_CustomerId",
                table: "CustomersBanks",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_deliveryAddresses_Communes_CommuneId",
                table: "deliveryAddresses",
                column: "CommuneId",
                principalTable: "Communes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_deliveryAddresses_Districts_DistrictId",
                table: "deliveryAddresses",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_deliveryAddresses_Provinces_ProvinceId",
                table: "deliveryAddresses",
                column: "ProvinceId",
                principalTable: "Provinces",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_deliveryAddresses_customers_CustomerId",
                table: "deliveryAddresses",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_PromotionDetails_ProductDetailId",
                table: "OrderDetails",
                column: "ProductDetailId",
                principalTable: "PromotionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_deliveryAddresses_DeliveryAddressId",
                table: "Orders",
                column: "DeliveryAddressId",
                principalTable: "deliveryAddresses",
                principalColumn: "Id");
        }
    }
}
