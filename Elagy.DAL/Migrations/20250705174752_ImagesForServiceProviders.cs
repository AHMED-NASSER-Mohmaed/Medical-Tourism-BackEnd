using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ImagesForServiceProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HospitalAssetImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HospitalAssetId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HospitalAssetImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HospitalAssetImages_HospitalAssets_HospitalAssetId",
                        column: x => x.HospitalAssetId,
                        principalTable: "HospitalAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HotelAssetImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HotelAssetId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelAssetImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelAssetImages_HotelAssets_HotelAssetId",
                        column: x => x.HotelAssetId,
                        principalTable: "HotelAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HospitalAssetImages_HospitalAssetId",
                table: "HospitalAssetImages",
                column: "HospitalAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelAssetImages_HotelAssetId",
                table: "HotelAssetImages",
                column: "HotelAssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HospitalAssetImages");

            migrationBuilder.DropTable(
                name: "HotelAssetImages");
        }
    }
}
