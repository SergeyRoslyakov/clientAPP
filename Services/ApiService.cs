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
        // Тестовые данные
        private List<DeviceModel> _testDevices = new List<DeviceModel>
        {
            new DeviceModel { Id = 1, Type = "Смартфон", Brand = "Apple", Model = "iPhone 15",
                        SerialNumber = "ABC123", ProblemDescription = "Не заряжается", ClientId = 1, ClientName = "Иванов И." },
            new DeviceModel { Id = 2, Type = "Ноутбук", Brand = "Dell", Model = "XPS 13",
                        SerialNumber = "XYZ789", ProblemDescription = "Не включается", ClientId = 2, ClientName = "Петров П." },
            new DeviceModel { Id = 3, Type = "Планшет", Brand = "Samsung", Model = "Galaxy Tab S9",
                        SerialNumber = "DEF456", ProblemDescription = "Разбит экран", ClientId = 1, ClientName = "Иванов И." }
        };

        public async Task<List<DeviceModel>> GetDevicesAsync(string search = null)
        {
            await Task.Delay(500);

            if (string.IsNullOrEmpty(search))
                return _testDevices;

            return _testDevices.Where(d =>
                d.Type.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                d.Brand.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                d.Model.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                d.SerialNumber.Contains(search, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public async Task<DeviceModel> GetDeviceAsync(int id)
        {
            await Task.Delay(300);
            return _testDevices.FirstOrDefault(d => d.Id == id) ?? new DeviceModel();
        }

        public async Task<DeviceModel> CreateDeviceAsync(CreateDeviceDto device)
        {
            await Task.Delay(500);
            var newDevice = new DeviceModel
            {
                Id = _testDevices.Count + 1,
                Type = device.Type,
                Brand = device.Brand,
                Model = device.Model,
                SerialNumber = device.SerialNumber,
                ProblemDescription = device.ProblemDescription,
                ClientId = device.ClientId,
                ClientName = $"Клиент {device.ClientId}"
            };

            _testDevices.Add(newDevice);
            return newDevice;
        }

        public async Task UpdateDeviceAsync(int id, UpdateDeviceDto device)
        {
            await Task.Delay(500);
            var existing = _testDevices.FirstOrDefault(d => d.Id == id);
            if (existing != null)
            {
                existing.Type = device.Type;
                existing.Brand = device.Brand;
                existing.Model = device.Model;
                existing.SerialNumber = device.SerialNumber;
                existing.ProblemDescription = device.ProblemDescription;
                existing.ClientId = device.ClientId;
            }
        }

        public async Task DeleteDeviceAsync(int id)
        {
            await Task.Delay(500);
            var device = _testDevices.FirstOrDefault(d => d.Id == id);
            if (device != null)
                _testDevices.Remove(device);
        }
    }
}