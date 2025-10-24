using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToBookings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_GymMemberId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GymMemberId_ClassSessionId",
                table: "Bookings",
                columns: new[] { "GymMemberId", "ClassSessionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_GymMemberId_ClassSessionId",
                table: "Bookings");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_GymMemberId",
                table: "Bookings",
                column: "GymMemberId");
        }
    }
}
