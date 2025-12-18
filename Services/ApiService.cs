using System.Text;
using System.Text.Json;
using clientAPP.Models;
using clientAPP.DTO;
using System.Diagnostics;
using clientAPP.Services;

namespace clientAPP.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:7212/api";
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService()
        {
            // Настройка JSON сериализатора
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };

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

                Debug.WriteLine($"GET запрос: {_baseUrl}{url}");
                var response = await _httpClient.GetAsync(url);

                Debug.WriteLine($"Статус ответа: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Ответ: {content}");

                var devices = JsonSerializer.Deserialize<List<DeviceModel>>(content, _jsonOptions);
                return devices ?? new List<DeviceModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка GetDevicesAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<DeviceModel> CreateDeviceAsync(CreateDeviceDto device)
        {
            try
            {
                var json = JsonSerializer.Serialize(device, _jsonOptions);
                Debug.WriteLine($"Отправляемый JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "/Device";
                Debug.WriteLine($"POST запрос: {_baseUrl}{url}");

                var response = await _httpClient.PostAsync(url, content);

                Debug.WriteLine($"Статус ответа: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Ошибка сервера: {errorContent}");
                    throw new HttpRequestException($"Server error: {response.StatusCode}. {errorContent}");
                }

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Ответ сервера: {responseContent}");

                return JsonSerializer.Deserialize<DeviceModel>(responseContent, _jsonOptions) ?? new DeviceModel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка CreateDeviceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateDeviceAsync(int id, UpdateDeviceDto device)
        {
            try
            {
                var json = JsonSerializer.Serialize(device, _jsonOptions);
                Debug.WriteLine($"Отправляемый JSON для обновления: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"/Device/{id}";
                Debug.WriteLine($"PUT запрос: {_baseUrl}{url}");

                var response = await _httpClient.PutAsync(url, content);

                Debug.WriteLine($"Статус ответа: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Ошибка сервера: {errorContent}");
                    throw new HttpRequestException($"Server error: {response.StatusCode}. {errorContent}");
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка UpdateDeviceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteDeviceAsync(int id)
        {
            try
            {
                var url = $"/Device/{id}";
                Debug.WriteLine($"DELETE запрос: {_baseUrl}{url}");

                var response = await _httpClient.DeleteAsync(url);

                Debug.WriteLine($"Статус ответа: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Ошибка сервера: {errorContent}");
                    throw new HttpRequestException($"Server error: {response.StatusCode}. {errorContent}");
                }

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка DeleteDeviceAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<DeviceModel> GetDeviceAsync(int id)
        {
            try
            {
                var url = $"/Device/{id}";
                Debug.WriteLine($"GET запрос для одного устройства: {_baseUrl}{url}");

                var response = await _httpClient.GetAsync(url);

                Debug.WriteLine($"Статус ответа: {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"Ответ: {content}");

                return JsonSerializer.Deserialize<DeviceModel>(content, _jsonOptions) ?? new DeviceModel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка GetDeviceAsync: {ex.Message}");
                throw;
            }
        }
    }
}