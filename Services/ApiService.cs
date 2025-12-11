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
        private const string BaseUrl = "https://localhost:7212/api";
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
            catch (Exception ex)
            {
                Console.WriteLine($"GetDevicesAsync error: {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"GetDeviceAsync error: {ex.Message}");
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
            catch (Exception ex)
            {
                Console.WriteLine($"CreateDeviceAsync error: {ex.Message}");
                return new DeviceModel();
            }
        }

        public async Task UpdateDeviceAsync(int id, UpdateDeviceDto device)
        {
            try
            {
                await SetAuthHeader();
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/devices/{id}", device);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"UpdateDeviceAsync failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateDeviceAsync error: {ex.Message}");
            }
        }

        public async Task DeleteDeviceAsync(int id)
        {
            try
            {
                await SetAuthHeader();
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/devices/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"DeleteDeviceAsync failed: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteDeviceAsync error: {ex.Message}");
            }
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