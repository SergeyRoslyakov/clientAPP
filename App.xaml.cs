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
            InitializeComponent();

            MainPage = new NavigationPage(new Pages.LoginPage());
        }
    }
}