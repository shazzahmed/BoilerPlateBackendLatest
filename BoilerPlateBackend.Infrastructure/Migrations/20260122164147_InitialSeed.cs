using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoilerPlateBackend.Infrastructure.Migrations
{
    public partial class InitialSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsSystem", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "00000000-0000-0000-0000-000000000000", false, "SuperAdmin", "SUPERADMIN" },
                    { "2", "00000000-0000-0000-0000-000000000000", true, "Admin", "ADMIN" },
                    { "3", "00000000-0000-0000-0000-000000000000", false, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Description", "Icon", "IsActive", "IsDashboard", "IsOpen", "Name", "OrderById", "ParentId", "Path", "Type" },
                values: new object[,]
                {
                    { 1, "", "fa fa-dashboard", false, false, false, "Dashboard", 1, null, "/dashboard", 0 },
                    { 2, "", "fe fe-settings", false, false, false, "Security", 2, null, "", 0 }
                });

            migrationBuilder.InsertData(
                table: "NotificationTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Email" },
                    { 2, "Sms" },
                    { 3, "Site" },
                    { 4, "Push" }
                });

            migrationBuilder.InsertData(
                table: "StatusTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "UserStatus" });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "TenantId", "Address", "City", "ContactEmail", "ContactPhone", "Country", "CreatedAt", "CreatedBy", "Currency", "CurrencySymbol", "CurrentStorageUsedGB", "CurrentUserCount", "DateFormat", "DeletedAt", "Domain", "EnabledFeatures", "IsActive", "IsDeleted", "IsSubscriptionValid", "Language", "Logo", "MaxUsers", "Name", "Notes", "OrganizationType", "PostalCode", "PrimaryAdminUserId", "PrimaryColor", "PrimaryContactName", "SecondaryColor", "ShortName", "State", "StorageLimitGB", "SubscriptionEndDate", "SubscriptionStartDate", "SubscriptionTier", "TenantCode", "TimeZone", "UpdatedAt", "UpdatedBy", "Website" },
                values: new object[] { 1, "Default Address", "Default City", "admin@organization.com", "03001234567", "Default Country", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "USD", "$", 0m, 0, null, null, null, null, true, false, true, null, null, 500, "Default Organization", null, null, null, null, null, null, null, "DO", "Default State", 10m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Premium", "TENANT001", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Description", "Icon", "IsActive", "IsDashboard", "IsOpen", "Name", "OrderById", "ParentId", "Path", "Type" },
                values: new object[,]
                {
                    { 101, "", "fa fa-user-circle-o", false, false, false, "Role", 1, 2, "/Security/Roles", 1 },
                    { 201, "", "fa fa-key", false, false, false, "Change Password", 1, 2, "/Security/ChangePassword", 2 },
                    { 202, "", "fe fe-user", false, false, false, "Profile", 2, 2, "/Security/Profile", 2 }
                });

            migrationBuilder.InsertData(
                table: "NotificationTemplates",
                columns: new[] { "Id", "Description", "MessageBody", "NotificationTypeId", "Subject" },
                values: new object[,]
                {
                    { 1, "Email confirmation when user is registered.", "Hi #Name </br></br>Thank you for registering in MultiServe. Click <a href=\"#Link\">here</a> to activate your account.", 1, "MultiServe Account Activation" },
                    { 2, "Confirmation sms when user is registered.", "Todo", 2, "" },
                    { 3, "Email, when user click on forget password.", "Hi #Name </br></br>Please click <a href=\"#Link\">here</a> to reset your password.", 1, "MultiServe Reset Password" },
                    { 4, "Code sent in sms, for forget password.", "Todo", 2, "" },
                    { 5, "Confirmation email when an external user set password.", "Hi #Name </br></br>Thank you for adding a local account in MultiServe. Click <a href=\"#Link\">here</a> to activate your account", 1, "MultiServe Account Activation" },
                    { 6, "Confirmation sms when an external user set password.", "Todo", 2, "" },
                    { 7, "Confirmation email when user change the password.", "Hi #Name </br></br>Click <a href=\"#Link\">here</a> to confirm your email to change it.</br></br> Thank you so much.", 1, "MultiServe Email Change" },
                    { 8, "Confirmation sms when user change the password.", "Todo", 2, "" },
                    { 9, "Code in email for tow factor authentication.", "Hi #Name </br></br>#Token is your code.</br></br> Thank you so much.", 1, "MultiServe Code" },
                    { 10, "Code in sms for tow factor authentication.", "Todo", 2, "" },
                    { 11, "Email to user whan user is block or status change to any status in application.", "Todo", 1, "MultiServe Account Restricted" },
                    { 12, "Sms to user whan user is block or status change to any status in application.", "Todo", 1, "" }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "ModuleId", "Name" },
                values: new object[,]
                {
                    { 1, "ADD permission for module Security and 2", 2, "ADD_Security" },
                    { 2, "EDIT permission for module Security and 2", 2, "EDIT_Security" },
                    { 3, "DELETE permission for module Security and 2", 2, "DELETE_Security" },
                    { 4, "VIEW permission for module Security and 2", 2, "VIEW_Security" },
                    { 5, "IMPORT permission for module Security and 2", 2, "IMPORT_Security" },
                    { 6, "EXPORT permission for module Security and 2", 2, "EXPORT_Security" },
                    { 7, "APPROVE permission for module Security and 2", 2, "APPROVE_Security" },
                    { 8, "ASSIGN permission for module Security and 2", 2, "ASSIGN_Security" }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "Code", "Name", "TypeId" },
                values: new object[,]
                {
                    { 1, "101", "Preactive", 1 },
                    { 2, "102", "Active", 1 },
                    { 3, "103", "Inactive", 1 },
                    { 4, "104", "Canceled", 1 },
                    { 5, "105", "Frozen", 1 },
                    { 6, "106", "Blocked", 1 }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "ModuleId", "Name" },
                values: new object[,]
                {
                    { 9, "ADD permission for module Role and 101", 101, "ADD_Role" },
                    { 10, "EDIT permission for module Role and 101", 101, "EDIT_Role" },
                    { 11, "DELETE permission for module Role and 101", 101, "DELETE_Role" },
                    { 12, "VIEW permission for module Role and 101", 101, "VIEW_Role" },
                    { 13, "IMPORT permission for module Role and 101", 101, "IMPORT_Role" },
                    { 14, "EXPORT permission for module Role and 101", 101, "EXPORT_Role" },
                    { 15, "APPROVE permission for module Role and 101", 101, "APPROVE_Role" },
                    { 16, "ASSIGN permission for module Role and 101", 101, "ASSIGN_Role" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 1, "1" },
                    { 2, "1" },
                    { 3, "1" },
                    { 4, "1" },
                    { 5, "1" },
                    { 6, "1" },
                    { 7, "1" },
                    { 8, "1" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { 9, "1" },
                    { 10, "1" },
                    { 11, "1" },
                    { 12, "1" },
                    { 13, "1" },
                    { 14, "1" },
                    { 15, "1" },
                    { 16, "1" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "NotificationTemplates",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 1, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 2, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 3, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 4, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 5, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 6, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 7, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 8, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 9, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 10, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 11, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 12, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 13, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 14, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 15, "1" });

            migrationBuilder.DeleteData(
                table: "RolePermission",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { 16, "1" });

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Statuses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "NotificationTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "StatusTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
