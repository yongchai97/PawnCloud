using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class addoutlet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "outletName",
                table: "GeneralSetups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "outletRegistrationNumber",
                table: "GeneralSetups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerOutlets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    Customer = table.Column<int>(type: "int", nullable: true),
                    GeneralSetup = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_CustomerOutlets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerOutlets_Customers_Customer",
                        column: x => x.Customer,
                        principalTable: "Customers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerOutlets_GeneralSetups_GeneralSetup",
                        column: x => x.GeneralSetup,
                        principalTable: "GeneralSetups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOutlets_Customer",
                table: "CustomerOutlets",
                column: "Customer");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOutlets_GeneralSetup",
                table: "CustomerOutlets",
                column: "GeneralSetup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOutlets");

            migrationBuilder.DropColumn(
                name: "outletName",
                table: "GeneralSetups");

            migrationBuilder.DropColumn(
                name: "outletRegistrationNumber",
                table: "GeneralSetups");
        }
    }
}
