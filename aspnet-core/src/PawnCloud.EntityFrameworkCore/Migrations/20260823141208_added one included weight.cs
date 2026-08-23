using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class addedoneincludedweight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "includedItemWeight",
                table: "PawnTickets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "includedItemWeight",
                table: "PawnTickets");
        }
    }
}
