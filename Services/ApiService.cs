using System.Text;
using System.Text.Json;
using clientAPP.Models;
using clientAPP.DTO;

namespace clientAPP.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7212/api"; // Базовый URL API
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiService()
        {
            // Настраиваем HttpClient для работы с самоподписанным сертификатом
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<List<DeviceModel>> GetDevicesAsync(string search = null)
        {
            try
            {
                var url = "/Device";
                if (!string.IsNullOrEmpty(search))
                {
                    url += $"?search={Uri.EscapeDataString(search)}";
                }

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var devices = JsonSerializer.Deserialize<List<DeviceModel>>(content, _jsonOptions);
                return devices ?? new List<DeviceModel>();
            }
            catch (HttpRequestException ex)
            {
                // Логируем ошибку
                Console.WriteLine($"Ошибка при получении устройств: {ex.Message}");
                throw;
            }
        }

        public async Task<DeviceModel> GetDeviceAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/Device/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var device = JsonSerializer.Deserialize<DeviceModel>(content, _jsonOptions);
                return device ?? new DeviceModel();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при получении устройства {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<DeviceModel> CreateDeviceAsync(CreateDeviceDto device)
        {
            try
            {
                var json = JsonSerializer.Serialize(device, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/Device", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DeviceModel>(responseContent, _jsonOptions) ?? new DeviceModel();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при создании устройства: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateDeviceAsync(int id, UpdateDeviceDto device)
        {
            try
            {
                var json = JsonSerializer.Serialize(device, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/Device/{id}", content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при обновлении устройства {id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteDeviceAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/Device/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при удалении устройства {id}: {ex.Message}");
                throw;
            }
        }
    }
}