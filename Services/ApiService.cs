using clientAPP.DTO;
using clientAPP.Models;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace clientAPP.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7212/api/Techno-Fix";
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<DeviceModel>> GetDevicesAsync(string search = null)
        {
            try
            {
                await SetAuthHeader();

                string url = $"{BaseUrl}/devices";
                if (!string.IsNullOrEmpty(search))
                {
                    url += $"?search={Uri.EscapeDataString(search)}";
                }

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<DeviceModel>>(_jsonOptions)
                        ?? new List<DeviceModel>();
                }

                return new List<DeviceModel>();
            }
            catch
            {
                return new List<DeviceModel>();
            }
        }

        public async Task<DeviceModel> GetDeviceAsync(int id)
        {
            try
            {
                await SetAuthHeader();
                var response = await _httpClient.GetAsync($"{BaseUrl}/devices/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DeviceModel>(_jsonOptions)
                        ?? new DeviceModel();
                }

                return new DeviceModel();
            }
            catch
            {
                return new DeviceModel();
            }
        }

        public async Task<DeviceModel> CreateDeviceAsync(CreateDeviceDto device)
        {
            try
            {
                await SetAuthHeader();
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/devices", device);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DeviceModel>(_jsonOptions)
                        ?? new DeviceModel();
                }

                return new DeviceModel();
            }
            catch
            {
                return new DeviceModel();
            }
        }

        public async Task UpdateDeviceAsync(int id, UpdateDeviceDto device)
        {
            try
            {
                await SetAuthHeader();
                await _httpClient.PutAsJsonAsync($"{BaseUrl}/devices/{id}", device);
            }
            catch { }
        }

        public async Task DeleteDeviceAsync(int id)
        {
            try
            {
                await SetAuthHeader();
                await _httpClient.DeleteAsync($"{BaseUrl}/devices/{id}");
            }
            catch { }
        }
        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { email, password };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(json);

                    if (jsonDoc.RootElement.TryGetProperty("token", out var tokenElement))
                    {
                        var token = tokenElement.GetString();
                        await SecureStorage.SetAsync("auth_token", token);
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            SecureStorage.Remove("auth_token");
        }

        public async Task<Models.User> GetCurrentUserAsync()
        {
            return new Models.User();
        }

        private async Task SetAuthHeader()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}