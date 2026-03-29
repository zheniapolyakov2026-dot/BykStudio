using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BykStudio.data.Migrations
{
    /// <inheritdoc />
    public partial class RoomCapacityRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Rooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Rooms",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Capacity",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Capacity",
                value: 15);
        }
    }
}
