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
                // Убираем авто-фильтрацию при вводе, будем обновлять при поиске
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
        public ICommand SearchCommand { get; }

        public DevicesViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Устройства";

            LoadDevicesCommand = new Command(async () => await LoadDevicesAsync());
            AddDeviceCommand = new Command(async () => await AddDeviceAsync());
            EditDeviceCommand = new Command<DeviceModel>(async (device) => await EditDeviceAsync(device));
            DeleteDeviceCommand = new Command<DeviceModel>(async (device) => await DeleteDeviceAsync(device));
            RefreshCommand = new Command(async () => await LoadDevicesAsync());
            SearchCommand = new Command(async () => await SearchDevicesAsync());

            // Загружаем устройства при старте
            Task.Run(async () => await LoadDevicesAsync());
        }

        private async Task LoadDevicesAsync()
        {
            await LoadOrSearchDevicesAsync(null);
        }

        private async Task SearchDevicesAsync()
        {
            await LoadOrSearchDevicesAsync(SearchText);
        }

        private async Task LoadOrSearchDevicesAsync(string? search)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                Devices.Clear();
                FilteredDevices.Clear();

                var devices = await _apiService.GetDevicesAsync(search);
                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        Devices.Add(device);
                        FilteredDevices.Add(device);
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Ошибка", $"Не удалось загрузить устройства: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
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

                // Запрашиваем ClientId
                var clientIdStr = await Application.Current.MainPage.DisplayPromptAsync(
                    "Новое устройство",
                    "ID клиента (число):",
                    keyboard: Keyboard.Numeric);

                if (string.IsNullOrWhiteSpace(clientIdStr) || !int.TryParse(clientIdStr, out int clientId) || clientId <= 0)
                {
                    await ShowAlert("Ошибка", "Введите корректный ID клиента (положительное число)");
                    return;
                }

                var type = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Тип устройства:");
                if (string.IsNullOrWhiteSpace(type))
                {
                    await ShowAlert("Ошибка", "Тип устройства обязателен");
                    return;
                }

                var brand = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Бренд:");
                if (string.IsNullOrWhiteSpace(brand))
                {
                    await ShowAlert("Ошибка", "Бренд обязателен");
                    return;
                }

                var model = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Модель:");
                if (string.IsNullOrWhiteSpace(model))
                {
                    await ShowAlert("Ошибка", "Модель обязательна");
                    return;
                }

                var serial = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Серийный номер:");
                var problem = await Application.Current.MainPage.DisplayPromptAsync("Новое устройство", "Описание проблемы:");

                var newDevice = new CreateDeviceDto
                {
                    Type = type,
                    Brand = brand,
                    Model = model,
                    SerialNumber = serial ?? "",
                    ProblemDescription = problem ?? "",
                    ClientId = clientId
                };

                await _apiService.CreateDeviceAsync(newDevice);
                await LoadDevicesAsync();
                await ShowAlert("Успех", "Устройство успешно создано!");
            }
            catch (Exception ex)
            {
                await ShowAlert("Ошибка", $"Не удалось создать устройство: {ex.Message}");
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

                // Запрашиваем ClientId
                var clientIdStr = await Application.Current.MainPage.DisplayPromptAsync(
                    "Редактировать",
                    "ID клиента:",
                    initialValue: device.ClientId.ToString(),
                    keyboard: Keyboard.Numeric);

                if (string.IsNullOrWhiteSpace(clientIdStr) || !int.TryParse(clientIdStr, out int clientId) || clientId <= 0)
                {
                    await ShowAlert("Ошибка", "Введите корректный ID клиента (положительное число)");
                    return;
                }

                var type = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Тип устройства:", initialValue: device.Type);
                if (string.IsNullOrWhiteSpace(type))
                {
                    await ShowAlert("Ошибка", "Тип устройства обязателен");
                    return;
                }

                var brand = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Бренд:", initialValue: device.Brand);
                if (string.IsNullOrWhiteSpace(brand))
                {
                    await ShowAlert("Ошибка", "Бренд обязателен");
                    return;
                }

                var model = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Модель:", initialValue: device.Model);
                if (string.IsNullOrWhiteSpace(model))
                {
                    await ShowAlert("Ошибка", "Модель обязательна");
                    return;
                }

                var serial = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Серийный номер:", initialValue: device.SerialNumber);
                var problem = await Application.Current.MainPage.DisplayPromptAsync("Редактировать", "Проблема:", initialValue: device.ProblemDescription);

                var updateDto = new UpdateDeviceDto
                {
                    Type = type,
                    Brand = brand,
                    Model = model,
                    SerialNumber = serial ?? "",
                    ProblemDescription = problem ?? "",
                    ClientId = clientId
                };

                await _apiService.UpdateDeviceAsync(device.Id, updateDto);
                await LoadDevicesAsync();
                await ShowAlert("Успех", "Устройство успешно обновлено!");
            }
            catch (Exception ex)
            {
                await ShowAlert("Ошибка", $"Не удалось обновить устройство: {ex.Message}");
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
                    await ShowAlert("Успех", "Устройство успешно удалено!");
                }
            }
            catch (Exception ex)
            {
                await ShowAlert("Ошибка", $"Не удалось удалить устройство: {ex.Message}");
            }
        }

        private async Task ShowAlert(string title, string message)
        {
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert(title, message, "OK");
                    });
                }
            }
            catch
            {
                // Игнорируем ошибки при показе алерта
            }
        }
    }
}