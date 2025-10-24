using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDateAndTimeFieldsToClassSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "ClassSessions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "ClassSessions",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "ClassSessions",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ClassSessions");
        }
    }
}
