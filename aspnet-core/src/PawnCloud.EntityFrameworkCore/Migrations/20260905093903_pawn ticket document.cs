using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class pawnticketdocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GeneralSetup",
                table: "PawnTickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PawnTicketDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    BlobName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    PawnTicket = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_PawnTicketDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PawnTicketDocuments_PawnTickets_PawnTicket",
                        column: x => x.PawnTicket,
                        principalTable: "PawnTickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PawnTickets_GeneralSetup",
                table: "PawnTickets",
                column: "GeneralSetup");

            migrationBuilder.CreateIndex(
                name: "IX_PawnTicketDocuments_PawnTicket",
                table: "PawnTicketDocuments",
                column: "PawnTicket");

            migrationBuilder.AddForeignKey(
                name: "FK_PawnTickets_GeneralSetups_GeneralSetup",
                table: "PawnTickets",
                column: "GeneralSetup",
                principalTable: "GeneralSetups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PawnTickets_GeneralSetups_GeneralSetup",
                table: "PawnTickets");

            migrationBuilder.DropTable(
                name: "PawnTicketDocuments");

            migrationBuilder.DropIndex(
                name: "IX_PawnTickets_GeneralSetup",
                table: "PawnTickets");

            migrationBuilder.DropColumn(
                name: "GeneralSetup",
                table: "PawnTickets");
        }
    }
}
