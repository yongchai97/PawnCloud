using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class redeem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "discount",
                table: "PawnTickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AuthorizedPeoples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Customer = table.Column<int>(type: "int", nullable: true),
                    NRIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    town = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    postcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    state = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<int>(type: "int", nullable: true),
                    telephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    relationship = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Race = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: true),
                    member = table.Column<bool>(type: "bit", nullable: false),
                    Nationality = table.Column<int>(type: "int", nullable: true),
                    birthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    age = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    blacklisted = table.Column<bool>(type: "bit", nullable: false),
                    oldIC = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_AuthorizedPeoples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizedPeoples_Customers_Customer",
                        column: x => x.Customer,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RedeemRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    redeemedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    redeemedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numberOfTickets = table.Column<int>(type: "int", nullable: false),
                    totalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalCustody = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalService = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    grandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                    table.PrimaryKey("PK_RedeemRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedeemRecordTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    PawnTicket = table.Column<int>(type: "int", nullable: true),
                    RedeemRecord = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_RedeemRecordTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedeemRecordTickets_PawnTickets_PawnTicket",
                        column: x => x.PawnTicket,
                        principalTable: "PawnTickets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RedeemRecordTickets_RedeemRecords_RedeemRecord",
                        column: x => x.RedeemRecord,
                        principalTable: "RedeemRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizedPeoples_Customer",
                table: "AuthorizedPeoples",
                column: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_RedeemRecordTickets_PawnTicket",
                table: "RedeemRecordTickets",
                column: "PawnTicket");

            migrationBuilder.CreateIndex(
                name: "IX_RedeemRecordTickets_RedeemRecord",
                table: "RedeemRecordTickets",
                column: "RedeemRecord");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizedPeoples");

            migrationBuilder.DropTable(
                name: "RedeemRecordTickets");

            migrationBuilder.DropTable(
                name: "RedeemRecords");

            migrationBuilder.DropColumn(
                name: "discount",
                table: "PawnTickets");
        }
    }
}
