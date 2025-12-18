using clientAPP.Pages;

namespace clientAPP
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Регистрируем маршруты
            Routing.RegisterRoute(nameof(LoginPage), typeof(Pages.LoginPage));
            Routing.RegisterRoute(nameof(DevicesPage), typeof(Pages.DevicesPage));

            // По умолчанию показываем страницу логина
            CurrentItem = Items[0];
        }
    }
}