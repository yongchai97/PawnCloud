using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawnCloud.Migrations
{
    /// <inheritdoc />
    public partial class newreason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "PawnRequestPermissions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reason",
                table: "PawnRequestPermissions");
        }
    }
}
