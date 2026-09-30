using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HallRental.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStartIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Start",
                table: "Bookings",
                column: "Start");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Bookings_Start",
                table: "Bookings");
        }
    }
}
