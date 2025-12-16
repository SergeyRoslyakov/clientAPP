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

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand NavigateToDevicesCommand { get; }

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Главная";

            LogoutCommand = new Command(Logout);
            NavigateToDevicesCommand = new Command(async () => await NavigateTo("DevicesPage"));
        }

        private void Logout()
        {
            _apiService.Logout();
            Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task NavigateTo(string page)
        {
            await Shell.Current.GoToAsync(page);
        }
    }
}
