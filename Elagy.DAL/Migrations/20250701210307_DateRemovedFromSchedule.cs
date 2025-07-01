using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DateRemovedFromSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Governaties_GovernateId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "CancelledSlots",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Schedules",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TimeSlotSize",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Governaties_GovernateId",
                table: "Assets",
                column: "GovernateId",
                principalTable: "Governaties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Governaties_GovernateId",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "CancelledSlots",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "TimeSlotSize",
                table: "Schedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Schedules",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Governaties_GovernateId",
                table: "Assets",
                column: "GovernateId",
                principalTable: "Governaties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
