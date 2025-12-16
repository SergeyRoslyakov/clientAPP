using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clientAPP.Services;
using clientAPP.Models;

namespace clientAPP.ViewModels
{
    public class DevicesViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private List<DeviceModel> _devices = new();

        public List<DeviceModel> Devices
        {
            get => _devices;
            set => SetProperty(ref _devices, value);
        }

        public Command LoadDevicesCommand { get; }
        public Command AddDeviceCommand { get; }

        public DevicesViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Устройства";

            LoadDevicesCommand = new Command(async () => await LoadDevicesAsync());
            AddDeviceCommand = new Command(async () => await AddDeviceAsync());
        }

        private async Task LoadDevicesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                Devices = await _apiService.GetDevicesAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddDeviceAsync()
        {
            // Реализовать добавление устройства
        }
    }
}
