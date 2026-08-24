using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class PawnTicketRectify2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_PawnTickets_GoldType",
                table: "PawnTickets");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_ItemListing",
                table: "PawnTickets");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_ItemStatus",
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
                name: "brand",
                table: "PawnTickets");

            migrationBuilder.RenameColumn(
                name: "length",
                table: "PawnTickets",
                newName: "serviceCharge");

            migrationBuilder.RenameColumn(
                name: "includedItems",
                table: "PawnTickets",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "includedItemWeight",
                table: "PawnTickets",
                newName: "monthlyCustody");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "PawnTickets",
                newName: "slotNumber");

            migrationBuilder.AddColumn<decimal>(
                name: "amount",
                table: "PawnTickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "expiryDate",
                table: "PawnTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "pledgedDate",
                table: "PawnTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "amount",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "expiryDate",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "pledgedDate",
                table: "PawnTickets");

            migrationBuilder.RenameColumn(
                name: "slotNumber",
                table: "PawnTickets",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "serviceCharge",
                table: "PawnTickets",
                newName: "length");

            migrationBuilder.RenameColumn(
                name: "monthlyCustody",
                table: "PawnTickets",
                newName: "includedItemWeight");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "PawnTickets",
                newName: "includedItems");

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

            migrationBuilder.AddColumn<string>(
                name: "brand",
                table: "PawnTickets",
                type: "nvarchar(max)",
                nullable: true);

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
    }
}
