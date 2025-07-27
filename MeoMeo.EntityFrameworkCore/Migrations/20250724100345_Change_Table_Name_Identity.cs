using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeoMeo.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Change_Table_Name_Identity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_permissionGroups_permissionGroups_ParentId",
                table: "permissionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_permissions_permissionGroups_PermissionGroupId",
                table: "permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_rolePermissions_permissions_PermissionId",
                table: "rolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_rolePermissions_roles_RoleId",
                table: "rolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_userRoles_Users_UserId",
                table: "userRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_userRoles_roles_RoleId",
                table: "userRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_userTokens_Users_UserId",
                table: "userTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_userTokens_Users_UserId1",
                table: "userTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userTokens",
                table: "userTokens");

            migrationBuilder.DropIndex(
                name: "IX_userTokens_UserId1",
                table: "userTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userRoles",
                table: "userRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_rolePermissions",
                table: "rolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_permissionGroups",
                table: "permissionGroups");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "userTokens");

            migrationBuilder.RenameTable(
                name: "userTokens",
                newName: "UserTokens");

            migrationBuilder.RenameTable(
                name: "userRoles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "rolePermissions",
                newName: "RolePermissions");

            migrationBuilder.RenameTable(
                name: "permissionGroups",
                newName: "PermissionGroups");

            migrationBuilder.RenameIndex(
                name: "IX_userTokens_UserId",
                table: "UserTokens",
                newName: "IX_UserTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_userRoles_RoleId",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_rolePermissions_RoleId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_rolePermissions_PermissionId",
                table: "RolePermissions",
                newName: "IX_RolePermissions_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_permissionGroups_ParentId",
                table: "PermissionGroups",
                newName: "IX_PermissionGroups_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PermissionGroups",
                table: "PermissionGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionGroups_PermissionGroups_ParentId",
                table: "PermissionGroups",
                column: "ParentId",
                principalTable: "PermissionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_permissions_PermissionGroups_PermissionGroupId",
                table: "permissions",
                column: "PermissionGroupId",
                principalTable: "PermissionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RolePermissions_permissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId",
                principalTable: "permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionGroups_PermissionGroups_ParentId",
                table: "PermissionGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_permissions_PermissionGroups_PermissionGroupId",
                table: "permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_Roles_RoleId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_RolePermissions_permissions_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTokens_Users_UserId",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTokens",
                table: "UserTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RolePermissions",
                table: "RolePermissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PermissionGroups",
                table: "PermissionGroups");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                newName: "userTokens");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "userRoles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "roles");

            migrationBuilder.RenameTable(
                name: "RolePermissions",
                newName: "rolePermissions");

            migrationBuilder.RenameTable(
                name: "PermissionGroups",
                newName: "permissionGroups");

            migrationBuilder.RenameIndex(
                name: "IX_UserTokens_UserId",
                table: "userTokens",
                newName: "IX_userTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "userRoles",
                newName: "IX_userRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_RoleId",
                table: "rolePermissions",
                newName: "IX_rolePermissions_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "rolePermissions",
                newName: "IX_rolePermissions_PermissionId");

            migrationBuilder.RenameIndex(
                name: "IX_PermissionGroups_ParentId",
                table: "permissionGroups",
                newName: "IX_permissionGroups_ParentId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "userTokens",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userTokens",
                table: "userTokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userRoles",
                table: "userRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_rolePermissions",
                table: "rolePermissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_permissionGroups",
                table: "permissionGroups",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_userTokens_UserId1",
                table: "userTokens",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_permissionGroups_permissionGroups_ParentId",
                table: "permissionGroups",
                column: "ParentId",
                principalTable: "permissionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_permissions_permissionGroups_PermissionGroupId",
                table: "permissions",
                column: "PermissionGroupId",
                principalTable: "permissionGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rolePermissions_permissions_PermissionId",
                table: "rolePermissions",
                column: "PermissionId",
                principalTable: "permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_rolePermissions_roles_RoleId",
                table: "rolePermissions",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userRoles_Users_UserId",
                table: "userRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userRoles_roles_RoleId",
                table: "userRoles",
                column: "RoleId",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userTokens_Users_UserId",
                table: "userTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userTokens_Users_UserId1",
                table: "userTokens",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
