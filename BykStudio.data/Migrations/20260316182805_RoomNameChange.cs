using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BykStudio.data.Migrations
{
    /// <inheritdoc />
    public partial class RoomNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Name",
                value: "зал 1");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Name",
                value: "зал 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "Name",
                value: "Room A");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Name",
                value: "Room B");
        }
    }
}
