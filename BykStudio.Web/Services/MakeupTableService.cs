using BykStudio.data.DTOs;

namespace BykStudio.Web.Services
{
    public class MakeupTableService
    {
        private readonly HttpClient _httpClient;

        public MakeupTableService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Returns the single makeup table (or null)
        public async Task<MakeupTableDto?> GetMakeupTableAsync()
        {
            var tables = await GetAllMakeupTablesAsync();
            return tables?.FirstOrDefault();
        }

        public async Task<List<MakeupTableDto>?> GetAllMakeupTablesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<MakeupTableDto>>("api/makeuptables");
            }
            catch
            {
                return null;
            }
        }
    }
}
