using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BykStudio.data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Photos",
                table: "Rooms",
                type: "text",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "Capacity", "Description", "IsAvailable", "MainImageUrl", "Name", "Photos", "PricePerHour" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), 10, "Spacious room with natural light", true, "/images/room7.jpg", "Room A", "[\"roomA_1.jpg\",\"roomA_2.jpg\"]", 1000m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), 15, "Equipped with professional gear", true, "/images/room7.jpg", "Room B", "[\"roomB_1.jpg\",\"roomB_2.jpg\"]", 1500m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.AlterColumn<List<string>>(
                name: "Photos",
                table: "Rooms",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
