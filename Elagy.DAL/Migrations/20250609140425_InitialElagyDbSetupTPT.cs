using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialElagyDbSetupTPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAssets_AspNetUsers_Id",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "AdditionalServices",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "CarFeatures",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "EmergencyServices",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "HasEmergencyRoom",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "HasPool",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "HasRestaurant",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "IsTeachingHospital",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "NumberOfDepartments",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "OperationalAreas",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "RentalPolicies",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "StarRating",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "Transmission",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                table: "ServiceAssets");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DOCsURL",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Docs",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "CarRentalAssets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OperationalAreas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Transmission = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentalPolicies = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalServices = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CarFeatures = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarRentalAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarRentalAssets_ServiceAssets_Id",
                        column: x => x.Id,
                        principalTable: "ServiceAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HospitalAssets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NumberOfDepartments = table.Column<int>(type: "int", nullable: false),
                    HasEmergencyRoom = table.Column<bool>(type: "bit", nullable: false),
                    IsTeachingHospital = table.Column<bool>(type: "bit", nullable: false),
                    EmergencyServices = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalAssets_ServiceAssets_Id",
                        column: x => x.Id,
                        principalTable: "ServiceAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelAssets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StarRating = table.Column<int>(type: "int", nullable: true),
                    HasPool = table.Column<bool>(type: "bit", nullable: true),
                    HasRestaurant = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelAssets_ServiceAssets_Id",
                        column: x => x.Id,
                        principalTable: "ServiceAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Patients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DOCsURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProviders_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceProviders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SuperAdmins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Docs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperAdmins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuperAdmins_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuperAdmins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_UserId",
                table: "Patients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviders_UserId",
                table: "ServiceProviders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SuperAdmins_UserId",
                table: "SuperAdmins",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAssets_ServiceProviders_Id",
                table: "ServiceAssets",
                column: "Id",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAssets_ServiceProviders_Id",
                table: "ServiceAssets");

            migrationBuilder.DropTable(
                name: "CarRentalAssets");

            migrationBuilder.DropTable(
                name: "HospitalAssets");

            migrationBuilder.DropTable(
                name: "HotelAssets");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "SuperAdmins");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalServices",
                table: "ServiceAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarFeatures",
                table: "ServiceAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmergencyServices",
                table: "ServiceAssets",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "ServiceAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasEmergencyRoom",
                table: "ServiceAssets",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPool",
                table: "ServiceAssets",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasRestaurant",
                table: "ServiceAssets",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTeachingHospital",
                table: "ServiceAssets",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDepartments",
                table: "ServiceAssets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OperationalAreas",
                table: "ServiceAssets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RentalPolicies",
                table: "ServiceAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StarRating",
                table: "ServiceAssets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Transmission",
                table: "ServiceAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleType",
                table: "ServiceAssets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DOCsURL",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Docs",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Height",
                table: "AspNetUsers",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Weight",
                table: "AspNetUsers",
                type: "real",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAssets_AspNetUsers_Id",
                table: "ServiceAssets",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
