using clientAPP.Models;
using clientAPP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace clientAPP.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private string _userName = string.Empty;
        private string _userRole = string.Empty;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string UserRole
        {
            get => _userRole;
            set => SetProperty(ref _userRole, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand NavigateToClientsCommand { get; }
        public ICommand NavigateToDevicesCommand { get; }

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Главная";

            LogoutCommand = new Command(Logout);
            NavigateToClientsCommand = new Command(async () => await NavigateTo("ClientsPage"));
            NavigateToDevicesCommand = new Command(async () => await NavigateTo("DevicesPage"));

            LoadUserData();
        }

        private async void LoadUserData()
        {
            var user = await _apiService.GetCurrentUserAsync();
            UserName = !string.IsNullOrEmpty(user.Username) ? user.Username : "Пользователь";
            UserRole = !string.IsNullOrEmpty(user.Role) ? user.Role : "User";
        }

        private void Logout()
        {
            _apiService.Logout();
            Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task NavigateTo(string page)
        {
            // Проверяем авторизацию перед навигацией
            var isAuthenticated = await _apiService.IsAuthenticatedAsync();
            if (isAuthenticated)
            {
                await Shell.Current.GoToAsync(page);
            }
            else
            {
                await Shell.Current.DisplayAlert("Ошибка", "Требуется авторизация", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
