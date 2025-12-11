using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clientAPP.Models;
using clientAPP.DTO;

namespace clientAPP.Services
{
    public interface IApiService
    {
        Task<List<DeviceModel>> GetDevicesAsync(string search = null);
        Task<DeviceModel> GetDeviceAsync(int id);
        Task<DeviceModel> CreateDeviceAsync(CreateDeviceDto device);
        Task UpdateDeviceAsync(int id, UpdateDeviceDto device);
        Task DeleteDeviceAsync(int id);
    }
}