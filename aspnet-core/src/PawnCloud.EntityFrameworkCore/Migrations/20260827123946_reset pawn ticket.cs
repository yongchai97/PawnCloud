using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class resetpawnticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "inputPrice",
                table: "DailyGoldPrices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "GeneralSetups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    serviceCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    maximumAllowedPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    monthsBetweenPledgeAndExpiry = table.Column<int>(type: "int", nullable: false),
                    ticketIdMethod = table.Column<int>(type: "int", nullable: true),
                    appendedString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppendYearMonth = table.Column<bool>(type: "bit", nullable: false),
                    appendedStringBackMethod = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_GeneralSetups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PawnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    pawnItemNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    PawnTicket = table.Column<int>(type: "int", nullable: true),
                    ItemListing = table.Column<int>(type: "int", nullable: true),
                    ItemStatus = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GoldType = table.Column<int>(type: "int", nullable: true),
                    weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncludedItem = table.Column<int>(type: "int", nullable: true),
                    includedItemWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    includedItemValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_PawnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PawnItems_GoldTypes_GoldType",
                        column: x => x.GoldType,
                        principalTable: "GoldTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PawnItems_ItemListings_ItemListing",
                        column: x => x.ItemListing,
                        principalTable: "ItemListings",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PawnItems_ItemStatuses_ItemStatus",
                        column: x => x.ItemStatus,
                        principalTable: "ItemStatuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PawnItems_PawnTickets_PawnTicket",
                        column: x => x.PawnTicket,
                        principalTable: "PawnTickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PawnItems_GoldType",
                table: "PawnItems",
                column: "GoldType");

            migrationBuilder.CreateIndex(
                name: "IX_PawnItems_ItemListing",
                table: "PawnItems",
                column: "ItemListing");

            migrationBuilder.CreateIndex(
                name: "IX_PawnItems_ItemStatus",
                table: "PawnItems",
                column: "ItemStatus");

            migrationBuilder.CreateIndex(
                name: "IX_PawnItems_PawnTicket",
                table: "PawnItems",
                column: "PawnTicket");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneralSetups");

            migrationBuilder.DropTable(
                name: "PawnItems");

            migrationBuilder.DropColumn(
                name: "inputPrice",
                table: "DailyGoldPrices");
        }
    }
}
