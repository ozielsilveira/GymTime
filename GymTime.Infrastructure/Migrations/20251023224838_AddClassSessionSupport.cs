using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTime.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddClassSessionSupport : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Bookings_Classes_ClassId",
            table: "Bookings");

        migrationBuilder.DropColumn(
            name: "Schedule",
            table: "Classes");

        migrationBuilder.AddColumn<Guid>(
            name: "ClassSessionId",
            table: "Bookings",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateTable(
            name: "ClassSessions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                Schedule = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClassSessions", x => x.Id);
                table.ForeignKey(
                    name: "FK_ClassSessions_Classes_ClassId",
                    column: x => x.ClassId,
                    principalTable: "Classes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_ClassSessionId",
            table: "Bookings",
            column: "ClassSessionId");

        migrationBuilder.CreateIndex(
            name: "IX_ClassSessions_ClassId",
            table: "ClassSessions",
            column: "ClassId");

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_ClassSessions_ClassSessionId",
            table: "Bookings",
            column: "ClassSessionId",
            principalTable: "ClassSessions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_Classes_ClassId",
            table: "Bookings",
            column: "ClassId",
            principalTable: "Classes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Bookings_ClassSessions_ClassSessionId",
            table: "Bookings");

        migrationBuilder.DropForeignKey(
            name: "FK_Bookings_Classes_ClassId",
            table: "Bookings");

        migrationBuilder.DropTable(
            name: "ClassSessions");

        migrationBuilder.DropIndex(
            name: "IX_Bookings_ClassSessionId",
            table: "Bookings");

        migrationBuilder.DropColumn(
            name: "ClassSessionId",
            table: "Bookings");

        migrationBuilder.AddColumn<DateTime>(
            name: "Schedule",
            table: "Classes",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddForeignKey(
            name: "FK_Bookings_Classes_ClassId",
            table: "Bookings",
            column: "ClassId",
            principalTable: "Classes",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
