using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CarRentalAppoinmentDataTimeChangedTobeDateOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndingDateTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StartingDateTime",
                table: "Appointments");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndingDate",
                table: "Appointments",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartingDate",
                table: "Appointments",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndingDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StartingDate",
                table: "Appointments");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndingDateTime",
                table: "Appointments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartingDateTime",
                table: "Appointments",
                type: "datetime2",
                nullable: true);
        }
    }
}
