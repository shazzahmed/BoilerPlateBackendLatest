using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMSBACKEND.Infrastructure.Migrations
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
                    { "3", "00000000-0000-0000-0000-000000000000", false, "User", "USER" },
                    { "4", "00000000-0000-0000-0000-000000000000", false, "Parent", "PARENT" },
                    { "5", "00000000-0000-0000-0000-000000000000", false, "Student", "STUDENT" },
                    { "6", "00000000-0000-0000-0000-000000000000", true, "Teacher", "TEACHER" },
                    { "7", "00000000-0000-0000-0000-000000000000", true, "Accountant", "ACCOUNTANT" },
                    { "8", "00000000-0000-0000-0000-000000000000", true, "Receptionist", "RECEPTIONIST" }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Description", "Icon", "IsActive", "IsDashboard", "IsOpen", "Name", "OrderById", "ParentId", "Path", "Type" },
                values: new object[,]
                {
                    { 1, "", "fa fa-mortar-board", false, false, false, "Academics", 2, null, "", 1 },
                    { 2, "", "fa fa-user-plus", false, false, false, "Student Information", 3, null, "", 1 },
                    { 3, "", "fa fa-dashboard", false, false, false, "Dashboard", 1, null, "/dashboard", 1 },
                    { 4, "", "fa fa-ioxhost ftlayer", false, false, false, "Front Office", 4, null, "", 0 },
                    { 5, "", "fa fa-money", false, false, false, "Fees Collection", 5, null, "", 0 },
                    { 6, "", "fa fa-sitemap", false, false, false, "Human Resource", 6, null, "", 0 },
                    { 7, "", "fa fa-calendar-check-o", false, false, false, "Attendance", 7, null, "", 1 },
                    { 8, "", "fa fa-usd", false, false, false, "Income", 8, null, "", 0 },
                    { 9, "", "fa fa-credit-card", false, false, false, "Expenses", 9, null, "", 0 },
                    { 10, "", "fa fa-map-o ftlayer", false, false, false, "Examinations", 10, null, "Exams", 1 },
                    { 11, "", "fe fe-settings", false, false, false, "Security", 11, null, "", 0 }
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
                columns: new[] { "TenantId", "AcademicYearEndMonth", "AcademicYearStartMonth", "Address", "AdmissionNoFormat", "AllowParentIdInCnicField", "City", "ContactEmail", "ContactPhone", "Country", "CreatedAt", "CreatedBy", "Currency", "CurrencySymbol", "CurrentStaffCount", "CurrentStorageUsedGB", "CurrentStudentCount", "DateFormat", "DeletedAt", "Domain", "EducationLevel", "EnabledFeatures", "InstitutionType", "IsActive", "IsDeleted", "IsSubscriptionValid", "Language", "LastStudentSequence", "Logo", "MaxStaff", "MaxStudents", "Notes", "PostalCode", "PrimaryAdminUserId", "PrimaryColor", "PrimaryContactName", "SchoolName", "SecondaryColor", "ShortName", "State", "StorageLimitGB", "SubscriptionEndDate", "SubscriptionStartDate", "SubscriptionTier", "TenantCode", "TimeZone", "UpdatedAt", "UpdatedBy", "Website" },
                values: new object[] { 1, 3, 4, "Default Address", "{SEQ}", true, "Default City", "admin@school.com", "03001234567", "Pakistan", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "PKR", "Rs", 0, 0m, 0, null, null, null, "All", null, "School", true, false, true, null, 0, null, 100, 1000, null, null, null, null, null, "ILMSuite School", null, "ISS", "Default State", 10m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Premium", "SCHOOL001", null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 1", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 2", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 3", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 4", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 5", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 6", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 7", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 8", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 9", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Class 10", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Play Group", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Nursery", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Kinder Garten", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "Department",
                columns: new[] { "Id", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Admin", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Academic", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Finance", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Exam", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Sports", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Library", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "FeeDiscount",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DiscountAmount", "DiscountPercentage", "DiscountType", "ExpiryDate", "IsActive", "IsDeleted", "IsRecurring", "IsSystem", "Name", "NoOfUseCount", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "sibling-disc", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 0m, 5m, 1, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, false, true, "Sibling Discount", 10, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, "early-bird-per", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 0m, 40m, 1, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, false, true, "Early Bird Discount Percentage", 10, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, "early-bird-amount", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 5000m, 0m, 0, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, false, true, "Early Bird Discount Amount", 10, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, "cls-top-disc", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 0m, 5m, 1, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, false, true, "Class Topper Discount", 10, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "FeeGroup",
                columns: new[] { "Id", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "IsSystem", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, true, "System", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, "System", null, false, true, false, "Tuition Fee", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, true, "Admission", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, true, "Registration", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, false, "Library", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, false, "Lab", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, false, "Annual", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, false, "Exam", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, false, "Sports", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, false, "I Installment", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, false, "II Installment", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "FeeType",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "FeeFrequency", "IsActive", "IsDeleted", "IsSystem", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "monthly-tuition-fee", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 4, true, false, true, "Monthly Tuition Fee", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, "admission-fee", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 0, true, false, false, "Admission Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, "library-fee", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 1, true, false, false, "Library Fee", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, "lab-charges", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 1, true, false, false, "Lab Charges", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 5, "annual-charges", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 1, true, false, false, "Annual Charges", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 6, "registration-charges", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 0, true, false, false, "Registration Fee", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 7, "security-fee", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 0, true, false, false, "Security Fee", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 8, "stationary-charges", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 1, true, false, false, "Stationary Charges", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "FeeType",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "FeeFrequency", "IsActive", "IsDeleted", "IsSystem", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 9, "exam-fee", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 1, true, false, false, "Exam Fee", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 10, "sports-fee", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 1, true, false, false, "Sports Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 11, "1-installment-fees", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 3, true, false, false, "1st Installment Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 12, "2-installment-fees", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 3, true, false, false, "2nd Installment Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 13, "3-installment-fees", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 3, true, false, false, "3rd Installment Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 14, "4-installment-fees", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 3, true, false, false, "4th Installment Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 15, "5-installment-fees", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 2, true, false, false, "5th Installment Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 16, "6-installment-fees", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, 2, true, false, false, "6th Installment Fees", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Description", "Icon", "IsActive", "IsDashboard", "IsOpen", "Name", "OrderById", "ParentId", "Path", "Type" },
                values: new object[,]
                {
                    { 101, "", "fa fa-table", false, false, false, "Sections", 1, 1, "/Academics/Sections", 2 },
                    { 102, "", "fa fa-book", false, false, false, "Classes", 2, 1, "/Academics/Classes", 2 },
                    { 103, "", "fa fa-clipboard", false, false, false, "Subjects", 3, 1, "/Academics/Subjects", 2 },
                    { 104, "", "fa fa-clipboard", false, false, false, "Teacher", 4, 1, "/Academics/Teachers", 2 },
                    { 105, "", "fa fa-clipboard", false, false, false, "Timetable", 5, 1, "/Academics/Timetable", 2 },
                    { 121, "", "fa fa-users", false, false, false, "Students", 1, 2, "/StudentsInfo/Students", 2 },
                    { 122, "", "fa fa-users", false, false, false, "Student Categories", 2, 2, "/StudentsInfo/Category", 2 },
                    { 123, "", "fa fa-users", false, false, false, "School House", 3, 2, "/StudentsInfo/SchoolHouse", 2 },
                    { 124, "", "fa fa-users", false, false, false, "Disable Reason", 4, 2, "/StudentsInfo/DisableReason", 2 },
                    { 141, "", "fa fa-calendar-check-o", false, false, false, "Fees Type", 1, 5, "/Fees/FeesType", 2 },
                    { 142, "", "fa fa-object-group", false, false, false, "Fees Group", 2, 5, "/Fees/FeesGroup", 2 },
                    { 143, "", "fa fa-object-group", false, false, false, "Fees Master", 3, 5, "/Fees/FeesMaster", 2 },
                    { 144, "", "fa fa-tag", false, false, false, "Fees Discount", 4, 5, "/Fees/FeesDiscount", 2 },
                    { 145, "", "fa fa-book", false, false, false, "Fee Assignment", 5, 5, "/Fees/FeeAssignment", 2 },
                    { 146, "", "fa fa-book", false, false, false, "Fee Transaction", 6, 5, "/Fees/FeeTransaction", 2 },
                    { 147, "", "fa fa-book", false, false, false, "Fine Waiver", 7, 5, "/Fees/FineWaiver", 2 },
                    { 148, "", "fa fa-book", false, false, false, "Payment Analytics", 8, 5, "/Fees/PaymentAnalytics", 2 },
                    { 161, "", "fa fa-user-circle-o", false, false, false, "Staff", 1, 6, "/HumanResource/Staff", 2 },
                    { 162, "", "fa fa-building", false, false, false, "Departments", 3, 6, "/HumanResource/Departments", 2 },
                    { 181, "", "fa fa-calendar-check-o", false, false, false, "Attendance", 1, 7, "/Attendance/StudentAttendance", 2 },
                    { 182, "", "fa fa-calendar-check-o", false, false, false, "Mark Attendance", 2, 7, "/Attendance/MarkStudentAttendance", 2 },
                    { 183, "", "fa fa-calendar-check-o", false, false, false, "Attendance Report", 3, 7, "/Attendance/Reports", 2 },
                    { 191, "", "fa fa-usd", false, false, false, "All Income", 1, 8, "/Income/AllIncome", 2 },
                    { 192, "", "fa fa-table", false, false, false, "Income Head", 2, 8, "/Income/IncomeHead", 2 },
                    { 201, "", "fa fa-credit-card", false, false, false, "All Expenses", 1, 9, "/Expenses/AllExpense", 2 },
                    { 202, "", "fa fa-table", false, false, false, "Expense Head", 2, 9, "/Expenses/ExpenseHead", 2 },
                    { 211, "", "fa fa-user-circle-o", false, false, false, "Role", 1, 11, "/Security/Roles", 2 },
                    { 231, "", "fa fa-info-circle", false, false, false, "Front Office Dashboard", 1, 4, "/FrontOffice/Dashboard", 2 },
                    { 232, "", "fa fa-info-circle", false, false, false, "Enquiry", 2, 4, "/FrontOffice/Enquiry", 2 },
                    { 233, "", "fa fa-info-circle", false, false, false, "Application", 3, 4, "/FrontOffice/Application", 2 },
                    { 234, "", "fa fa-info-circle", false, false, false, "Entry Test", 4, 4, "/FrontOffice/EntryTest", 2 },
                    { 235, "", "fa fa-info-circle", false, false, false, "Admission", 5, 4, "/FrontOffice/Admission", 2 },
                    { 251, "", "fa fa-info-circle", false, false, false, "Exam Management", 1, 10, "/Exams/Management", 2 },
                    { 252, "", "fa fa-info-circle", false, false, false, "Exam Marks", 2, 10, "/Exams/Marks", 2 }
                });

            migrationBuilder.InsertData(
                table: "Modules",
                columns: new[] { "Id", "Description", "Icon", "IsActive", "IsDashboard", "IsOpen", "Name", "OrderById", "ParentId", "Path", "Type" },
                values: new object[,]
                {
                    { 253, "", "fa fa-info-circle", false, false, false, "Exam Results", 3, 10, "/Exams/Results", 2 },
                    { 254, "", "fa fa-info-circle", false, false, false, "Grade Configuration", 4, 10, "/Exams/GradeConfiguration", 2 },
                    { 501, "", "", false, false, false, "Import Student", 1, 2, "", 3 },
                    { 502, "", "", false, false, false, "Student Timeline", 2, 2, "", 3 },
                    { 503, "", "fa fa-key", false, false, false, "Change Password", 3, 11, "/Security/ChangePassword", 3 },
                    { 504, "", "fe fe-user", false, false, false, "Profile", 4, 11, "/Security/Profile", 3 }
                });

            migrationBuilder.InsertData(
                table: "NotificationTemplates",
                columns: new[] { "Id", "Description", "MessageBody", "NotificationTypeId", "Subject" },
                values: new object[,]
                {
                    { 1, "Email confirmation when user is registered.", "Hi #Name </br></br>Thank you for registering in ItSolution. Click <a href=\"#Link\">here</a> to activate your account.", 1, "ItSolution Account Activation" },
                    { 2, "Confirmation sms when user is registered.", "Todo", 2, "" },
                    { 3, "Email, when user click on forget password.", "Hi #Name </br></br>Please click <a href=\"#Link\">here</a> to reset your password.", 1, "ItSolution Reset Password" },
                    { 4, "Code sent in sms, for forget password.", "Todo", 2, "" },
                    { 5, "Confirmation email when an external user set password.", "Hi #Name </br></br>Thank you for adding a local account in ItSolution. Click <a href=\"#Link\">here</a> to activate your account", 1, "ItSolution Account Activation" },
                    { 6, "Confirmation sms when an external user set password.", "Todo", 2, "" },
                    { 7, "Confirmation email when user change the password.", "Hi #Name </br></br>Click <a href=\"#Link\">here</a> to confirm your email to change it.</br></br> Thank you so much.", 1, "ItSolution Email Change" },
                    { 8, "Confirmation sms when user change the password.", "Todo", 2, "" },
                    { 9, "Code in email for tow factor authentication.", "Hi #Name </br></br>#Token is your code.</br></br> Thank you so much.", 1, "ItSolution Code" },
                    { 10, "Code in sms for tow factor authentication.", "Todo", 2, "" },
                    { 11, "Email to user whan user is block or status change to any status in application.", "Todo", 1, "ItSolution Account Restricted" },
                    { 12, "Sms to user whan user is block or status change to any status in application.", "Todo", 1, "" }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "ModuleId", "Name" },
                values: new object[,]
                {
                    { 1, "ADD permission for module Security and 11", 11, "ADD_Security" },
                    { 2, "EDIT permission for module Security and 11", 11, "EDIT_Security" },
                    { 3, "DELETE permission for module Security and 11", 11, "DELETE_Security" },
                    { 4, "VIEW permission for module Security and 11", 11, "VIEW_Security" },
                    { 5, "IMPORT permission for module Security and 11", 11, "IMPORT_Security" },
                    { 6, "EXPORT permission for module Security and 11", 11, "EXPORT_Security" },
                    { 7, "APPROVE permission for module Security and 11", 11, "APPROVE_Security" },
                    { 8, "ASSIGN permission for module Security and 11", 11, "ASSIGN_Security" }
                });

            migrationBuilder.InsertData(
                table: "SchoolHouse",
                columns: new[] { "Id", "Color", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "#FF0000", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Red", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, "#FFFF00", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Yellow", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, "#00FF00", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Green", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, "#0000FF", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Blue", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "A", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "B", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "C", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "Session",
                columns: new[] { "Id", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "EndDate", "IsActive", "IsDeleted", "Sessions", "StartDate", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2021, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2020-2021", new DateTime(2020, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2022, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2021-2022", new DateTime(2021, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2023, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2022-2023", new DateTime(2022, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2024, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2023-2024", new DateTime(2023, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2025, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2024-2025", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2026, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "2025-2026", new DateTime(2025, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2027, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2026-2027", new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2028, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2027-2028", new DateTime(2027, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2029, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2028-2029", new DateTime(2028, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "Session",
                columns: new[] { "Id", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "EndDate", "IsActive", "IsDeleted", "Sessions", "StartDate", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, new DateTime(2030, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), false, false, "2029-2030", new DateTime(2029, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" });

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
                table: "StudentCategory",
                columns: new[] { "Id", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "IsActive", "IsDeleted", "Name", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "General", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Special", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, null, true, false, "Physically Challenged", "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "Subject",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Name", "SyncStatus", "TenantId", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "English", "synced", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, "002", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Math", "synced", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, "003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Physics", "synced", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, "004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Science", "synced", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 5, "005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Urdu", "synced", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 6, "006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Social Studies", "synced", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 7, "007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Computer", "synced", 1, 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 8, "003P", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Physics Practical", "synced", 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 9, "007P", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, "Computer Practical", "synced", 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "ClassSections",
                columns: new[] { "Id", "ClassId", "CreatedAt", "CreatedAtUtc", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "SectionId", "SyncStatus", "TenantId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 2, 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 3, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 4, 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 5, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 6, 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 7, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 8, 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 9, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 10, 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 11, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 12, 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 13, 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 14, 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 15, 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 16, 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 17, 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 18, 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 19, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 20, 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 21, 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 22, 11, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 23, 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 24, 12, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 25, 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 1, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" },
                    { 26, 13, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, true, false, 2, "synced", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System" }
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "ModuleId", "Name" },
                values: new object[,]
                {
                    { 9, "ADD permission for module Role and 211", 211, "ADD_Role" },
                    { 10, "EDIT permission for module Role and 211", 211, "EDIT_Role" },
                    { 11, "DELETE permission for module Role and 211", 211, "DELETE_Role" },
                    { 12, "VIEW permission for module Role and 211", 211, "VIEW_Role" },
                    { 13, "IMPORT permission for module Role and 211", 211, "IMPORT_Role" },
                    { 14, "EXPORT permission for module Role and 211", 211, "EXPORT_Role" },
                    { 15, "APPROVE permission for module Role and 211", 211, "APPROVE_Role" },
                    { 16, "ASSIGN permission for module Role and 211", 211, "ASSIGN_Role" }
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
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8");

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "ClassSections",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FeeDiscount",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FeeDiscount",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FeeDiscount",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FeeDiscount",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "FeeGroup",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "FeeType",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 121);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 122);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 123);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 124);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 141);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 142);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 143);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 144);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 145);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 146);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 147);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 148);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 161);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 162);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 181);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 182);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 183);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 191);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 192);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 201);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 202);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 231);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 232);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 233);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 234);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 235);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 251);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 252);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 253);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 254);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 501);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 502);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 503);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 504);

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
                table: "SchoolHouse",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SchoolHouse",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SchoolHouse",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SchoolHouse",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Session",
                keyColumn: "Id",
                keyValue: 10);

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
                table: "StudentCategory",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "StudentCategory",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "StudentCategory",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Subject",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Classes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 10);

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
                table: "Sections",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sections",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "StatusTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 211);

            migrationBuilder.DeleteData(
                table: "Tenants",
                keyColumn: "TenantId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Modules",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}
