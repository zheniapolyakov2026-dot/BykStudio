using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BykStudio.data.Migrations
{
    /// <inheritdoc />
    public partial class RoomNameChange1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "Description", "Name", "Photos" },
                values: new object[] { "Зал | \r\n\r\nЗал в стиле минимализм. \r\nВ ваше пользование будет предоставлено:\r\n- циклорама \r\n- проф.оборудование \r\n- флаги\r\n- 2-х метровый кожаный диван\r\n- бумажные фоны\r\n- тканевые фоны \r\n- кресло \r\n- 4 стула \r\n- черная кожаная банкетка   \r\n- зеркало\r\n- рейл\r\n в данном зале блэкаут шторы", "Зал 1", "[\"/images/room7.jpg\",\"/images/room7.jpg\"]" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Name",
                value: "Зал 2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "Description", "Name", "Photos" },
                values: new object[] { "Spacious room with natural light", "зал 1", "[\"room7.jpg\",\"room7.jpg\"]" });

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "RoomId",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Name",
                value: "зал 2");
        }
    }
}
