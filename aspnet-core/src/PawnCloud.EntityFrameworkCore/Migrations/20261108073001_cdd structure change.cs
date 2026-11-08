using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class cddstructurechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BusinessNature",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PEPStatus",
                table: "CustomerCDDs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "appearInPrivateBlackList",
                table: "CustomerCDDs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "appearInPublicBlackList",
                table: "CustomerCDDs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "highRiskCountry",
                table: "CustomerCDDs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "mediumRiskCountry",
                table: "CustomerCDDs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "missingTransactionPurpose",
                table: "CustomerCDDs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "noSourceOfFund",
                table: "CustomerCDDs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "partyName",
                table: "CustomerCDDs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "roleInParty",
                table: "CustomerCDDs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transactionPurpose",
                table: "CustomerCDDs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PEPStatus",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "appearInPrivateBlackList",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "appearInPublicBlackList",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "highRiskCountry",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "mediumRiskCountry",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "missingTransactionPurpose",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "noSourceOfFund",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "partyName",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "roleInParty",
                table: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "transactionPurpose",
                table: "CustomerCDDs");

            migrationBuilder.AlterColumn<int>(
                name: "BusinessNature",
                table: "Customers",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
