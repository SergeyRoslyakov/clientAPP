using clientAPP.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using clientAPP.Services;

namespace clientAPP.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            var apiService = new ApiService();
            BindingContext = new LoginViewModel(apiService);
        }
    }
}