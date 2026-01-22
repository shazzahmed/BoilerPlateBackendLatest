using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMSBACKEND.Infrastructure.Migrations
{
    public partial class AddInitialFeePackageTrackingToFeeAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InitialPackageId",
                table: "FeeAssignment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInitialAssignment",
                table: "FeeAssignment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialPackageId",
                table: "FeeAssignment");

            migrationBuilder.DropColumn(
                name: "IsInitialAssignment",
                table: "FeeAssignment");
        }
    }
}
