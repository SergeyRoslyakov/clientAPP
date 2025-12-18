using System.Text;
using System.Text.Json;
using clientAPP.Models;
using clientAPP.DTO;

namespace clientAPP.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7212";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            // Логирование для отладки
            Console.WriteLine($"API Service initialized with base URL: {_baseUrl}");
        }

        public async Task<List<DeviceModel>> GetDevicesAsync(string search = null)
        {
            try
            {
                var url = "/api/Device"; // Проверьте правильный путь
                if (!string.IsNullOrEmpty(search))
                {
                    url += $"?search={Uri.EscapeDataString(search)}";
                }

                Console.WriteLine($"GET request to: {_baseUrl}{url}");
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GET response: {content}");

                var devices = JsonSerializer.Deserialize<List<DeviceModel>>(content, _jsonOptions);
                return devices ?? new List<DeviceModel>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка GET devices: {ex.Message}");
                throw;
            }
        }

        public async Task<DeviceModel> GetDeviceAsync(int id)
        {
            try
            {
                var url = $"/api/Device/{id}";
                Console.WriteLine($"GET request to: {_baseUrl}{url}");

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GET response for device {id}: {content}");

                return JsonSerializer.Deserialize<DeviceModel>(content, _jsonOptions) ?? new DeviceModel();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка GET device {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<DeviceModel> CreateDeviceAsync(CreateDeviceDto device)
        {
            try
            {
                var url = "/api/Device"; // Проверьте этот URL
                Console.WriteLine($"POST request to: {_baseUrl}{url}");
                Console.WriteLine($"POST data: {JsonSerializer.Serialize(device)}");

                var json = JsonSerializer.Serialize(device, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                // Проверяем статус код
                Console.WriteLine($"POST response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"POST error response: {errorContent}");
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
                }

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"POST success response: {responseContent}");

                return JsonSerializer.Deserialize<DeviceModel>(responseContent, _jsonOptions) ?? new DeviceModel();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка POST device: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateDeviceAsync(int id, UpdateDeviceDto device)
        {
            try
            {
                var url = $"/api/Device/{id}";
                Console.WriteLine($"PUT request to: {_baseUrl}{url}");
                Console.WriteLine($"PUT data: {JsonSerializer.Serialize(device)}");

                var json = JsonSerializer.Serialize(device, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);

                Console.WriteLine($"PUT response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"PUT error response: {errorContent}");
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
                }

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка PUT device {id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteDeviceAsync(int id)
        {
            try
            {
                var url = $"/api/Device/{id}";
                Console.WriteLine($"DELETE request to: {_baseUrl}{url}");

                var response = await _httpClient.DeleteAsync(url);

                Console.WriteLine($"DELETE response status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"DELETE error response: {errorContent}");
                    throw new HttpRequestException($"HTTP {response.StatusCode}: {errorContent}");
                }

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка DELETE device {id}: {ex.Message}");
                throw;
            }
        }
    }
}