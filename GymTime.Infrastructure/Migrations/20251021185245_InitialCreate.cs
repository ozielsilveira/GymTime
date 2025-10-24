using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymTime.Infrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Classes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ClassType = table.Column<string>(type: "text", nullable: false),
                Schedule = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                MaxCapacity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Classes", x => x.Id));

        migrationBuilder.CreateTable(
            name: "GymMembers",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                PlanType = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_GymMembers", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Bookings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                GymMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Bookings", x => x.Id);
                table.ForeignKey(
                    name: "FK_Bookings_Classes_ClassId",
                    column: x => x.ClassId,
                    principalTable: "Classes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Bookings_GymMembers_GymMemberId",
                    column: x => x.GymMemberId,
                    principalTable: "GymMembers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_ClassId",
            table: "Bookings",
            column: "ClassId");

        migrationBuilder.CreateIndex(
            name: "IX_Bookings_GymMemberId",
            table: "Bookings",
            column: "GymMemberId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Bookings");

        migrationBuilder.DropTable(
            name: "Classes");

        migrationBuilder.DropTable(
            name: "GymMembers");
    }
}
