using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class CarRentalAppintmentModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarRentalScheduleId",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "CarId",
                table: "CarRentalSchedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndingDate",
                table: "CarRentalSchedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartingDate",
                table: "CarRentalSchedules",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalSchedules_CarId",
                table: "CarRentalSchedules",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarRentalScheduleId",
                table: "Appointments",
                column: "CarRentalScheduleId",
                principalTable: "CarRentalSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarRentalSchedules_Cars_CarId",
                table: "CarRentalSchedules",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarRentalScheduleId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_CarRentalSchedules_Cars_CarId",
                table: "CarRentalSchedules");

            migrationBuilder.DropIndex(
                name: "IX_CarRentalSchedules_CarId",
                table: "CarRentalSchedules");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "CarRentalSchedules");

            migrationBuilder.DropColumn(
                name: "EndingDate",
                table: "CarRentalSchedules");

            migrationBuilder.DropColumn(
                name: "StartingDate",
                table: "CarRentalSchedules");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarRentalScheduleId",
                table: "Appointments",
                column: "CarRentalScheduleId",
                principalTable: "CarRentalSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
