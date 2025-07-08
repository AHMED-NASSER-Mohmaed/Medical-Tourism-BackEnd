using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class imagespecialty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarRentalScheduleId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "CarRentalScheduleId",
                table: "Appointments",
                newName: "CarScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_CarRentalScheduleId",
                table: "Appointments",
                newName: "IX_Appointments_CarScheduleId");

            migrationBuilder.AddColumn<string>(
                name: "ImageId",
                table: "Specialties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageURL",
                table: "Specialties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarScheduleId",
                table: "Appointments",
                column: "CarScheduleId",
                principalTable: "CarRentalSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarScheduleId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Specialties");

            migrationBuilder.DropColumn(
                name: "ImageURL",
                table: "Specialties");

            migrationBuilder.RenameColumn(
                name: "CarScheduleId",
                table: "Appointments",
                newName: "CarRentalScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_CarScheduleId",
                table: "Appointments",
                newName: "IX_Appointments_CarRentalScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_CarRentalSchedules_CarRentalScheduleId",
                table: "Appointments",
                column: "CarRentalScheduleId",
                principalTable: "CarRentalSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
