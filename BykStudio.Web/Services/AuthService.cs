using System.Text.Json;
using BykStudio.data.DTOs;
using Microsoft.JSInterop;

namespace BykStudio.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<RegistrationResult?> RegisterAsync(Register model)
        {
            var resp = await _http.PostAsJsonAsync("api/Auth/register", model);
            if (resp.IsSuccessStatusCode)
            {
                var response = await resp.Content.ReadFromJsonAsync<RegisterResponse>();
                return new RegistrationResult { Success = true, Response = response };
            }
            else
            {
                var errorBody = await resp.Content.ReadAsStringAsync();
                var errors = new List<string>();

                try
                {
                    // Identity returns a JSON array of errors: [ { "code": "...", "description": "..." } ]
                    var json = JsonDocument.Parse(errorBody).RootElement;
                    if (json.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var item in json.EnumerateArray())
                        {
                            // Prefer the "description" field (user-friendly message)
                            if (item.TryGetProperty("description", out var desc))
                            {
                                errors.Add(desc.GetString() ?? "Unknown error");
                            }
                            else if (item.TryGetProperty("code", out var code))
                            {
                                errors.Add($"Error code: {code}");
                            }
                        }
                    }
                    else
                    {
                        errors.Add(errorBody); // fallback
                    }
                }
                catch
                {
                    errors.Add("Registration failed. Please try again.");
                }

                return new RegistrationResult { Success = false, Errors = errors };
            }
        }

        public async Task<LoginResult> LoginAsync(Login model)
        {
            var resp = await _http.PostAsJsonAsync("api/Auth/login", model);
            if (resp.IsSuccessStatusCode)
            {
                var login = await resp.Content.ReadFromJsonAsync<LoginResponse>();
                if (login?.Token != null)
                    await _js.InvokeVoidAsync("localStorage.setItem", "authToken", login.Token);
                return new LoginResult { Success = true, Token = login?.Token };
            }
            else
            {
                // Try to extract error message from response
                var errorBody = await resp.Content.ReadAsStringAsync();
                var errors = new List<string>();

                try
                {
                    var json = JsonDocument.Parse(errorBody).RootElement;
                    // If it's a simple { "message": "..." } format
                    if (json.TryGetProperty("message", out var msg))
                    {
                        errors.Add(msg.GetString() ?? "Login failed");
                    }
                    else if (json.ValueKind == JsonValueKind.Array)
                    {
                        // Identity error array
                        foreach (var item in json.EnumerateArray())
                        {
                            if (item.TryGetProperty("description", out var desc))
                                errors.Add(desc.GetString() ?? "Unknown error");
                            else if (item.TryGetProperty("code", out var code))
                                errors.Add($"Error code: {code}");
                        }
                    }
                    else
                    {
                        errors.Add(errorBody);
                    }
                }
                catch
                {
                    errors.Add("Invalid login attempt.");
                }

                return new LoginResult { Success = false, Errors = errors };
            }
        }

        public async Task LogoutAsync() => await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTokenAsync error: {ex}");
                return null;
            }
        }
    }
}
