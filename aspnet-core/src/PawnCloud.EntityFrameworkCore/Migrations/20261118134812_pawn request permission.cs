using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class pawnrequestpermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PawnRequestPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    TicketNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer = table.Column<int>(type: "int", nullable: true),
                    request = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    approvalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requestedBy = table.Column<int>(type: "int", nullable: true),
                    approvedBy = table.Column<int>(type: "int", nullable: true),
                    newTicketNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_PawnRequestPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PawnRequestPermissions_Customers_Customer",
                        column: x => x.Customer,
                        principalTable: "Customers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PawnRequestPermissions_Customer",
                table: "PawnRequestPermissions",
                column: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PawnRequestPermissions");
        }
    }
}
