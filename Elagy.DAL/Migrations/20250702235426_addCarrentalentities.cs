using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addCarrentalentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarRentalAssetId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Doctor_YearsOfExperience",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriveLicenseLicenseNumberId",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriveLicenseLicenseNumberURL",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CarRentalAssetImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CarRentalAssetId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarRentalAssetImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarRentalAssetImages_CarRentalAssets_CarRentalAssetId",
                        column: x => x.CarRentalAssetId,
                        principalTable: "CarRentalAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FactoryMake = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModelYear = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    PricePerDay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Transmission = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CarRentalAssetId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cars_CarRentalAssets_CarRentalAssetId",
                        column: x => x.CarRentalAssetId,
                        principalTable: "CarRentalAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelSchedule_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarDrivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarRentalAssetId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AssignmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsAssignedCurrent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarDrivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarDrivers_AspNetUsers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CarDrivers_CarRentalAssets_CarRentalAssetId",
                        column: x => x.CarRentalAssetId,
                        principalTable: "CarRentalAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarDrivers_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarImages_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CarRentalAssetId",
                table: "AspNetUsers",
                column: "CarRentalAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDrivers_CarId_DriverId",
                table: "CarDrivers",
                columns: new[] { "CarId", "DriverId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarDrivers_CarRentalAssetId",
                table: "CarDrivers",
                column: "CarRentalAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_CarDrivers_DriverId",
                table: "CarDrivers",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CarImages_CarId",
                table: "CarImages",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarRentalAssetImages_CarRentalAssetId",
                table: "CarRentalAssetImages",
                column: "CarRentalAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarRentalAssetId",
                table: "Cars",
                column: "CarRentalAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelSchedule_RoomId",
                table: "HotelSchedule",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_CarRentalAssets_CarRentalAssetId",
                table: "AspNetUsers",
                column: "CarRentalAssetId",
                principalTable: "CarRentalAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_CarRentalAssets_CarRentalAssetId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CarDrivers");

            migrationBuilder.DropTable(
                name: "CarImages");

            migrationBuilder.DropTable(
                name: "CarRentalAssetImages");

            migrationBuilder.DropTable(
                name: "HotelSchedule");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CarRentalAssetId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CarRentalAssetId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Doctor_YearsOfExperience",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DriveLicenseLicenseNumberId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DriveLicenseLicenseNumberURL",
                table: "AspNetUsers");
        }
    }
}
