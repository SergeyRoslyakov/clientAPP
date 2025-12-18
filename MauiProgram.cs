using clientAPP.Services;
using clientAPP.ViewModels;
using clientAPP.Pages;
using Microsoft.Extensions.Logging;

namespace clientAPP;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Регистрация сервисов
        builder.Services.AddSingleton<IApiService, ApiService>();

        // Регистрация ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<DevicesViewModel>();

        // Регистрация страниц
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<DevicesPage>();

        return builder.Build();
    }
}