using clientAPP.Services;
using clientAPP.Models;
using clientAPP.DTO;

namespace clientAPP
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            MainPage = new AppShell();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Регистрируем API сервис
            services.AddSingleton<IApiService, ApiService>();

            // Регистрируем ViewModels
            services.AddTransient<DeviceModel>();

            // Если есть другие страницы
            // services.AddTransient<LoginViewModel>();
            // services.AddTransient<MainViewModel>();
        }

        // Хелпер для получения сервисов
        public static T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }
    }
}