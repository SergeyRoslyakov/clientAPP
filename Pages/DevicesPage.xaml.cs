using clientAPP.ViewModels;
using clientAPP.Services;
using Microsoft.Maui.Controls;

namespace clientAPP.Pages
{
    public partial class DevicesPage : ContentPage
    {
        public DevicesPage()
        {
            InitializeComponent();

            var apiService = new ApiService();

            Task.Run(async () =>
            {
                var isAuth = await apiService.IsAuthenticated();
                if (!isAuth)
                {
                    await Shell.Current.GoToAsync("//login");
                }
                else
                {
                    // Авторизован, загружаем устройства
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        BindingContext = new DevicesViewModel(apiService);
                    });
                }
            });
        }
    }
}