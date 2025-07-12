using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class rateforcarrental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StarRating",
                table: "CarRentalAssets",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StarRating",
                table: "CarRentalAssets");
        }
    }
}
