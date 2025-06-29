using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Elagy.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addScheduletimeanddays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Schedules",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "DayOfWeekId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DayOfWeeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShortCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayOfWeeks", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DayOfWeeks",
                columns: new[] { "Id", "Name", "ShortCode" },
                values: new object[,]
                {
                    { 1, "Sunday", "SUN" },
                    { 2, "Monday", "MON" },
                    { 3, "Tuesday", "TUE" },
                    { 4, "Wednesday", "WED" },
                    { 5, "Thursday", "THU" },
                    { 6, "Friday", "FRI" },
                    { 7, "Saturday", "SAT" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_DayOfWeekId",
                table: "Schedules",
                column: "DayOfWeekId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_DayOfWeeks_DayOfWeekId",
                table: "Schedules",
                column: "DayOfWeekId",
                principalTable: "DayOfWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_DayOfWeeks_DayOfWeekId",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "DayOfWeeks");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_DayOfWeekId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "DayOfWeekId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Schedules");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
