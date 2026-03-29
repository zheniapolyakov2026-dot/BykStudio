using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BykStudio.data.Migrations
{
    /// <inheritdoc />
    public partial class addedNumberOfPeopleBookingM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfPeople",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPeople",
                table: "Bookings");
        }
    }
}
