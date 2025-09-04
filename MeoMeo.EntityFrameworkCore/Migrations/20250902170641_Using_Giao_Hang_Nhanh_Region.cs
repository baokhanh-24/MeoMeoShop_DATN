using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Using_Giao_Hang_Nhanh_Region : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
{

    migrationBuilder.DropIndex(
        name: "IX_DeliveryAddresses_CommuneId",
        table: "DeliveryAddresses");

    migrationBuilder.DropIndex(
        name: "IX_DeliveryAddresses_DistrictId",
        table: "DeliveryAddresses");

    migrationBuilder.DropIndex(
        name: "IX_DeliveryAddresses_ProvinceId",
        table: "DeliveryAddresses");

    // 2. Thêm cột mới kiểu int
    migrationBuilder.AddColumn<int>(
        name: "ProvinceIdInt",
        table: "DeliveryAddresses",
        type: "int",
        nullable: true);

    migrationBuilder.AddColumn<int>(
        name: "DistrictIdInt",
        table: "DeliveryAddresses",
        type: "int",
        nullable: true);

    migrationBuilder.AddColumn<int>(
        name: "CommuneIdInt",
        table: "DeliveryAddresses",
        type: "int",
        nullable: true);

    // 3. Nếu có mapping Guid -> Int thì copy dữ liệu ở đây
    // migrationBuilder.Sql("UPDATE DeliveryAddresses SET ProvinceIdInt = ... FROM ...");

    // 4. Xóa cột cũ
    migrationBuilder.DropColumn(name: "ProvinceId", table: "DeliveryAddresses");
    migrationBuilder.DropColumn(name: "DistrictId", table: "DeliveryAddresses");
    migrationBuilder.DropColumn(name: "CommuneId", table: "DeliveryAddresses");

    // 5. Đổi tên cột mới thành tên cũ
    migrationBuilder.RenameColumn(name: "ProvinceIdInt", table: "DeliveryAddresses", newName: "ProvinceId");
    migrationBuilder.RenameColumn(name: "DistrictIdInt", table: "DeliveryAddresses", newName: "DistrictId");
    migrationBuilder.RenameColumn(name: "CommuneIdInt", table: "DeliveryAddresses", newName: "CommuneId");

    // 6. Tạo index/FK mới (nếu bảng Provinces/Districts/Communes đã có khóa chính kiểu int)
    migrationBuilder.CreateIndex(
        name: "IX_DeliveryAddresses_ProvinceId",
        table: "DeliveryAddresses",
        column: "ProvinceId");

    migrationBuilder.CreateIndex(
        name: "IX_DeliveryAddresses_DistrictId",
        table: "DeliveryAddresses",
        column: "DistrictId");

    migrationBuilder.CreateIndex(
        name: "IX_DeliveryAddresses_CommuneId",
        table: "DeliveryAddresses",
        column: "CommuneId");
}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ProvinceId",
                table: "DeliveryAddresses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "DistrictId",
                table: "DeliveryAddresses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "CommuneId",
                table: "DeliveryAddresses",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddresses_CommuneId",
                table: "DeliveryAddresses",
                column: "CommuneId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddresses_DistrictId",
                table: "DeliveryAddresses",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddresses_ProvinceId",
                table: "DeliveryAddresses",
                column: "ProvinceId");

            
        }
    }
}
