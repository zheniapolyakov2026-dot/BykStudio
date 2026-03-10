using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BykStudio.data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomContent2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Photos",
                value: "[\"room7.jpg\",\"room7.jpg\"]");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Photos",
                value: "[\"room7.jpg\",\"room7.jpg\"]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Photos",
                value: "[\"roomA_1.jpg\",\"roomA_2.jpg\"]");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Photos",
                value: "[\"roomB_1.jpg\",\"roomB_2.jpg\"]");
        }
    }
}
