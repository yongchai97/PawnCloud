using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class addedcountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    countryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    countryCodeTwoAlphabet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    countryCodeThreeAlphabet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sequenceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    riskLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    help = table.Column<bool>(type: "bit", nullable: false),
                    effectiveYear = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
