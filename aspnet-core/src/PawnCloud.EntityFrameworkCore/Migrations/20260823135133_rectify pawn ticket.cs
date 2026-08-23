using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class rectifypawnticket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PawnTickets_Customers_CustomerId",
                table: "PawnTickets");

            migrationBuilder.DropTable(
                name: "PawnItems");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_CustomerId",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "MaturityDate",
                table: "PawnTickets");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "PawnTickets",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "PawnTickets",
                newName: "brand");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "PawnTickets",
                newName: "includedItems");

            migrationBuilder.AddColumn<int>(
                name: "Customer",
                table: "PawnTickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GoldType",
                table: "PawnTickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemListing",
                table: "PawnTickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemStatus",
                table: "PawnTickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "length",
                table: "PawnTickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "value",
                table: "PawnTickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "weight",
                table: "PawnTickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_PawnTickets_Customer",
                table: "PawnTickets",
                column: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_PawnTickets_GoldType",
                table: "PawnTickets",
                column: "GoldType");

            migrationBuilder.CreateIndex(
                name: "IX_PawnTickets_ItemListing",
                table: "PawnTickets",
                column: "ItemListing");

            migrationBuilder.CreateIndex(
                name: "IX_PawnTickets_ItemStatus",
                table: "PawnTickets",
                column: "ItemStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_PawnTickets_Customers_Customer",
                table: "PawnTickets",
                column: "Customer",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PawnTickets_GoldTypes_GoldType",
                table: "PawnTickets",
                column: "GoldType",
                principalTable: "GoldTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PawnTickets_ItemListings_ItemListing",
                table: "PawnTickets",
                column: "ItemListing",
                principalTable: "ItemListings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PawnTickets_ItemStatuses_ItemStatus",
                table: "PawnTickets",
                column: "ItemStatus",
                principalTable: "ItemStatuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PawnTickets_Customers_Customer",
                table: "PawnTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_PawnTickets_GoldTypes_GoldType",
                table: "PawnTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_PawnTickets_ItemListings_ItemListing",
                table: "PawnTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_PawnTickets_ItemStatuses_ItemStatus",
                table: "PawnTickets");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_Customer",
                table: "PawnTickets");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_GoldType",
                table: "PawnTickets");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_ItemListing",
                table: "PawnTickets");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_ItemStatus",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "Customer",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "GoldType",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "ItemListing",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "ItemStatus",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "length",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "value",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "weight",
                table: "PawnTickets");

            migrationBuilder.RenameColumn(
                name: "includedItems",
                table: "PawnTickets",
                newName: "BranchId");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "PawnTickets",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "brand",
                table: "PawnTickets",
                newName: "Remarks");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PawnTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "PawnTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "PawnTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MaturityDate",
                table: "PawnTickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PawnItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PawnTicketId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true),
                    LoanValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MarketValue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Purity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PawnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PawnItems_PawnTickets_PawnTicketId",
                        column: x => x.PawnTicketId,
                        principalTable: "PawnTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PawnTickets_CustomerId",
                table: "PawnTickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PawnItems_PawnTicketId",
                table: "PawnItems",
                column: "PawnTicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_PawnTickets_Customers_CustomerId",
                table: "PawnTickets",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
