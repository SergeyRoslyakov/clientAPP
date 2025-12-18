using System.Collections.Generic;
using System.Threading.Tasks;
using clientAPP.Models;
using clientAPP.DTO;

namespace clientAPP.Services
{
    public interface IApiService
    {
        Task<User> LoginAsync(LoginDto loginDto);
        Task<User> RegisterAsync(RegisterDto registerDto);
        Task<bool> IsAuthenticated();

        Task<List<DeviceModel>> GetDevicesAsync(string search = null);
        Task<DeviceModel> GetDeviceAsync(int id);
        Task<DeviceModel> CreateDeviceAsync(CreateDeviceDto device);
        Task UpdateDeviceAsync(int id, UpdateDeviceDto device);
        Task DeleteDeviceAsync(int id);
        string GetToken();
        void SetToken(string token);
        void ClearToken();
        Task<bool> TestApiConnectionAsync();
    }
}