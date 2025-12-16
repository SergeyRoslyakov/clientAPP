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

        // Аутентификация
        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginRequestDto { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions);

                    if (authResponse != null && !string.IsNullOrEmpty(authResponse.Token))
                    {
                        // Сохраняем токен и данные пользователя
                        await SecureStorage.SetAsync("auth_token", authResponse.Token);
                        await SecureStorage.SetAsync("user_email", authResponse.User.Email);
                        await SecureStorage.SetAsync("user_role", authResponse.User.Role);
                        await SecureStorage.SetAsync("user_name", authResponse.User.Username);

                        return authResponse;
                    }
                }

                // Если ошибка, возвращаем пустой объект
                return new AuthResponseDto();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
                return new AuthResponseDto();
            }
        }

        public async Task<AuthResponseDto> RegisterAsync(string username, string email, string password, string role = "User")
        {
            try
            {
                var registerDto = new RegisterRequestDto
                {
                    Username = username,
                    Email = email,
                    Password = password,
                    Role = role
                };

                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/auth/register", registerDto);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponseDto>(_jsonOptions)
                        ?? new AuthResponseDto();
                }

                return new AuthResponseDto();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
                return new AuthResponseDto();
            }
        }

        public async Task<User> GetCurrentUserAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");
                var email = await SecureStorage.GetAsync("user_email");
                var role = await SecureStorage.GetAsync("user_role");
                var name = await SecureStorage.GetAsync("user_name");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
                {
                    return new User
                    {
                        Email = email,
                        Role = role ?? "User",
                        Username = name ?? "Пользователь"
                    };
                }

                return new User();
            }
            catch
            {
                return new User();
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }

        public void Logout()
        {
            try
            {
                SecureStorage.Remove("auth_token");
                SecureStorage.Remove("user_email");
                SecureStorage.Remove("user_role");
                SecureStorage.Remove("user_name");
            }
            catch { }
        }

        // Устройства (с авторизацией)
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
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Если не авторизован
                    await Shell.Current.DisplayAlert("Ошибка", "Требуется авторизация", "OK");
                }

                return new List<DeviceModel>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetDevices error: {ex.Message}");
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

        private async Task SetAuthHeader()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}