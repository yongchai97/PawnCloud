using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class addedCDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "amountPerGram",
                table: "PawnTickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "IncludedItem",
                table: "PawnItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerCDDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Customer = table.Column<int>(type: "int", nullable: true),
                    highNetWorth = table.Column<bool>(type: "bit", nullable: false),
                    businessSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    businessType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    publicResearchCompany = table.Column<bool>(type: "bit", nullable: false),
                    researchDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    allowAnomaly = table.Column<bool>(type: "bit", nullable: false),
                    measureCustomer = table.Column<bool>(type: "bit", nullable: false),
                    offerUnusualTransaction = table.Column<bool>(type: "bit", nullable: false),
                    nomineeService = table.Column<bool>(type: "bit", nullable: false),
                    nomineeCustomer = table.Column<bool>(type: "bit", nullable: false),
                    crossBorderCustomer = table.Column<bool>(type: "bit", nullable: false),
                    PaymentMode = table.Column<int>(type: "int", nullable: true),
                    DeliveryChannel = table.Column<int>(type: "int", nullable: true),
                    UNSCRMatching = table.Column<bool>(type: "bit", nullable: false),
                    MOHAMatching = table.Column<bool>(type: "bit", nullable: false),
                    otherMatching = table.Column<bool>(type: "bit", nullable: false),
                    matchingID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approval = table.Column<bool>(type: "bit", nullable: false),
                    approvedBy = table.Column<int>(type: "int", nullable: true),
                    matchingDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_CustomerCDDs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCDDs_Customers_Customer",
                        column: x => x.Customer,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCDDs_Customer",
                table: "CustomerCDDs",
                column: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerCDDs");

            migrationBuilder.DropColumn(
                name: "amountPerGram",
                table: "PawnTickets");

            migrationBuilder.AlterColumn<int>(
                name: "IncludedItem",
                table: "PawnItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
