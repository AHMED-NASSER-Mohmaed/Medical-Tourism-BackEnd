using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StatusAddedToCarRentalSchedul : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CarRentalSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CarRentalSchedules");
        }
    }
}
