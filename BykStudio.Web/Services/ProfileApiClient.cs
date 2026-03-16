using System.Net.Http.Headers;

namespace BykStudio.Web.Services
{
    public class ProfileApiClient
    {
        public HttpClient HttpClient { get; }
        public ProfileApiClient(HttpClient httpClient) => HttpClient = httpClient;

        public async Task<HttpResponseMessage> GetProfileAsync(string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Auth/profile");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await HttpClient.SendAsync(request);
        }
    }
}
