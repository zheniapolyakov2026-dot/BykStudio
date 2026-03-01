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
            // Assumes your API has an endpoint like: api/rooms/byName/{name}
            return await _httpClient.GetFromJsonAsync<Room>($"api/rooms/byName/{name}");
        }

        // Alternative: fetch all rooms and filter locally
        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Room>>("api/rooms")
                   ?? [];
        }
    }
}
