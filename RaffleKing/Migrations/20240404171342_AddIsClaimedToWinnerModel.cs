using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaffleKing.Migrations
{
    /// <inheritdoc />
    public partial class AddIsClaimedToWinnerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClaimed",
                table: "Winner",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClaimed",
                table: "Winner");
        }
    }
}
