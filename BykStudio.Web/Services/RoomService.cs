using BykStudio.data.DTOs;
using BykStudio.data.Models;

namespace BykStudio.Web.Services
{
    public class RoomService
    {
        private readonly HttpClient _httpClient;

        public RoomService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get room by name (case‑insensitive)
        public async Task<Room?> GetRoomByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            // Uses the endpoint we already know works (/api/rooms)
            var allRooms = await GetAllRoomsAsync();
            return allRooms.FirstOrDefault(r =>
                string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        // Alternative: fetch all rooms and filter locally
        public async Task<List<Room>?> GetAllRoomsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/rooms");
                Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response body: {content}");

                response.EnsureSuccessStatusCode();

                var rooms = await response.Content.ReadFromJsonAsync<List<Room>>();
                return rooms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex}");
                return null;
            }
        }

        public async Task<RoomDto?> GetRoomDtoByNameAsync(string name)
        {
            var rooms = await GetAllRoomDtosAsync();
            return rooms?.FirstOrDefault(r =>
                string.Equals(r.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<RoomDto>?> GetAllRoomDtosAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<RoomDto>>("api/rooms");
                return response;
            }
            catch
            {
                return null;
            }
        }
    }
}
