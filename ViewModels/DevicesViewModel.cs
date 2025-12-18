using System.Collections.ObjectModel;
using System.Windows.Input;
using clientAPP.Models;
using clientAPP.DTO;
using clientAPP.Services;

namespace clientAPP.ViewModels
{
    public class DevicesViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private string _searchText = string.Empty;
        private DeviceModel _selectedDevice;

        public ObservableCollection<DeviceModel> Devices { get; } = new();
        public ObservableCollection<DeviceModel> FilteredDevices { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterDevices();
            }
        }

        public DeviceModel SelectedDevice
        {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public ICommand LoadDevicesCommand { get; }
        public ICommand AddDeviceCommand { get; }
        public ICommand EditDeviceCommand { get; }
        public ICommand DeleteDeviceCommand { get; }
        public ICommand RefreshCommand { get; }

        public DevicesViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Устройства";

            LoadDevicesCommand = new Command(async () => await LoadDevicesAsync());
            AddDeviceCommand = new Command(async () => await AddDeviceAsync());
            EditDeviceCommand = new Command<DeviceModel>(async (device) => await EditDeviceAsync(device));
            DeleteDeviceCommand = new Command<DeviceModel>(async (device) => await DeleteDeviceAsync(device));
            RefreshCommand = new Command(async () => await LoadDevicesAsync());

            // Загружаем устройства при старте
            Task.Run(async () => await LoadDevicesAsync());
        }

        private async Task LoadDevicesAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                Devices.Clear();
                FilteredDevices.Clear();

                var devices = await _apiService.GetDevicesAsync(SearchText);

                foreach (var device in devices)
                {
                    Devices.Add(device);
                    FilteredDevices.Add(device);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Ошибка", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void FilterDevices()
        {
            FilteredDevices.Clear();

            var filtered = string.IsNullOrEmpty(SearchText)
                ? Devices
                : Devices.Where(d =>
                    d.Type.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    d.Brand.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    d.Model.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    d.SerialNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var device in filtered)
            {
                FilteredDevices.Add(device);
            }
        }

        private async Task AddDeviceAsync()
        {
            try
            {
                if (Application.Current?.MainPage == null)
                {
                    System.Diagnostics.Debug.WriteLine("MainPage недоступен");
                    return;
                }

                var type = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Тип устройства:");
                if (string.IsNullOrWhiteSpace(type)) return;

                var brand = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Бренд:");
                if (string.IsNullOrWhiteSpace(brand)) return;

                var model = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Модель:");
                if (string.IsNullOrWhiteSpace(model)) return;

                var serial = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Серийный номер:");
                var problem = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Описание проблемы:");

                var newDevice = new CreateDeviceDto
                {
                    Type = type,
                    Brand = brand,
                    Model = model,
                    SerialNumber = serial ?? "",
                    ProblemDescription = problem ?? "",
                    ClientId = 1
                };

                await _apiService.CreateDeviceAsync(newDevice);
                await LoadDevicesAsync();
            }
            catch (Exception ex)
            {
                ShowAlert("Ошибка", ex.Message);
            }
        }

        private async Task EditDeviceAsync(DeviceModel device)
        {
            try
            {
                if (device == null) return;

                if (Application.Current?.MainPage == null)
                {
                    System.Diagnostics.Debug.WriteLine("MainPage недоступен");
                    return;
                }

                var type = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Тип устройства:", initialValue: device.Type);
                if (string.IsNullOrWhiteSpace(type)) return;

                var brand = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Бренд:", initialValue: device.Brand);
                if (string.IsNullOrWhiteSpace(brand)) return;

                var model = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Модель:", initialValue: device.Model);
                if (string.IsNullOrWhiteSpace(model)) return;

                var serial = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Серийный номер:", initialValue: device.SerialNumber);
                var problem = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Проблема:", initialValue: device.ProblemDescription);

                var updateDto = new UpdateDeviceDto
                {
                    Type = type,
                    Brand = brand,
                    Model = model,
                    SerialNumber = serial ?? "",
                    ProblemDescription = problem ?? "",
                    ClientId = device.ClientId
                };

                await _apiService.UpdateDeviceAsync(device.Id, updateDto);
                await LoadDevicesAsync();
            }
            catch (Exception ex)
            {
                ShowAlert("Ошибка", ex.Message);
            }
        }

        private async Task DeleteDeviceAsync(DeviceModel device)
        {
            try
            {
                if (device == null) return;

                if (Application.Current?.MainPage == null)
                {
                    System.Diagnostics.Debug.WriteLine("MainPage недоступен");
                    return;
                }

                var confirm = await Application.Current.MainPage.DisplayAlert(
                    "Удаление",
                    $"Удалить устройство {device.Brand} {device.Model}?",
                    "Удалить", "Отмена");

                if (confirm)
                {
                    await _apiService.DeleteDeviceAsync(device.Id);
                    await LoadDevicesAsync();
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Ошибка", ex.Message);
            }
        }

        private void ShowAlert(string title, string message)
        {
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
                    });
                }
            }
            catch
            {

            }
        }
    }
}