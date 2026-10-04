using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class generalsetuptable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "GeneralSetupTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    GeneralSetup = table.Column<int>(type: "int", nullable: true),
                    effectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    maximumPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    firstMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    secondMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    thirdMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    fourthMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    fifthMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sixthMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    seventhMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    eighthMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ninthMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    tenthMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    eleventhMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    twelfthMonthInterestRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneralSetupTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralSetupTables_GeneralSetups_GeneralSetup",
                        column: x => x.GeneralSetup,
                        principalTable: "GeneralSetups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneralSetupTables_GeneralSetup",
                table: "GeneralSetupTables",
                column: "GeneralSetup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSetupTables");

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
        }
    }
}
