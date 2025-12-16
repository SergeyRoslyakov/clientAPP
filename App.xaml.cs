using clientAPP.Services;
using clientAPP.Models;
using clientAPP.DTO;
using clientAPP.Services;
using clientAPP.ViewModels;
using clientAPP.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace clientAPP
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();

                var services = new ServiceCollection();
                services.AddSingleton<Services.IApiService, Services.ApiService>();
                services.AddSingleton<ViewModels.LoginViewModel>();
                services.AddSingleton<ViewModels.MainViewModel>();
                services.AddTransient<ViewModels.ClientsViewModel>();
                services.AddTransient<ViewModels.DevicesViewModel>();
                ServiceProvider = services.BuildServiceProvider();

                MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                MainPage = new ContentPage
                {
                    Content = new VerticalStackLayout
                    {
                        Children =
                        {
                            new Label { Text = $"Ошибка при запуске: {ex.Message}" },
                            new Label { Text = ex.StackTrace }
                        }
                    }
                };
            }
        }

        public static IServiceProvider ServiceProvider { get; private set; }
    }
}