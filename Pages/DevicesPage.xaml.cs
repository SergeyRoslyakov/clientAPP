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

            // Создаем ViewModel вручную
            var apiService = new ApiService();
            BindingContext = new DevicesViewModel(apiService);
        }
    }
}