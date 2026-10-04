using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class detailedgeneralsetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "bandarayaLicenseExpiryDate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "bandarayaLicenseLastUpdate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "insuranceExpiryDate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "insuranceLastUpdate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "insurancePolicyNumber",
                table: "GeneralSetups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "interestOption",
                table: "GeneralSetups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "interestRate",
                table: "GeneralSetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "interestRate2",
                table: "GeneralSetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "interestRate3",
                table: "GeneralSetups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "kpktLicenseExpiryDate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "kpktLicenseLastUpdate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "kpktPermitIklanExpiryDate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "kpktPermitIklanLastUpdate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "outletAddress",
                table: "GeneralSetups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "outletCity",
                table: "GeneralSetups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "outletCountry",
                table: "GeneralSetups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "outletPostcode",
                table: "GeneralSetups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "outletState",
                table: "GeneralSetups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "pdpaExpiryDate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "pdpaLastUpdate",
                table: "GeneralSetups",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "mailingPostcode",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "postcode",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bandarayaLicenseExpiryDate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "bandarayaLicenseLastUpdate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "insuranceExpiryDate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "insuranceLastUpdate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "insurancePolicyNumber",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "interestOption",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "interestRate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "interestRate2",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "interestRate3",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "kpktLicenseExpiryDate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "kpktLicenseLastUpdate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "kpktPermitIklanExpiryDate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "kpktPermitIklanLastUpdate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "outletAddress",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "outletCity",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "outletCountry",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "outletPostcode",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "outletState",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "pdpaExpiryDate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "pdpaLastUpdate",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "mailingPostcode",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "postcode",
                table: "Customers");
        }
    }
}
