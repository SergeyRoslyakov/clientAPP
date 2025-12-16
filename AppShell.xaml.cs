using clientAPP.Services;

namespace clientAPP
{
    public partial class AppShell : Shell
    {
        private readonly IApiService _apiService;

        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("LoginPage", typeof(Pages.LoginPage));
            Routing.RegisterRoute("MainPage", typeof(Pages.MainPage));
            //Routing.RegisterRoute("DevicesPage", typeof(Pages.DevicesPage));
            Routing.RegisterRoute("ClientsPage", typeof(Pages.ClientsPage));

            this.Navigated += OnNavigated;
        }

        private async void OnNavigated(object sender, ShellNavigatedEventArgs e)
        {
            if (e.Current?.Location?.OriginalString != "//LoginPage")
            {
                var apiService = App.ServiceProvider.GetService<Services.IApiService>();
                var isAuthenticated = await apiService?.IsAuthenticatedAsync();

                if (!isAuthenticated)
                {
                    await GoToAsync("//LoginPage");
                }
            }
        }
    }
}