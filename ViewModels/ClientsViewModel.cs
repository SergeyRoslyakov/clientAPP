using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using clientAPP.Services;
using clientAPP.Models;


namespace clientAPP.ViewModels
{
    public class ClientsViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;

        public Command LoadDataCommand { get; }

        public ClientsViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Клиенты";

            LoadDataCommand = new Command(async () => await LoadDataAsync());
        }

        private async Task LoadDataAsync()
        {
            // Здесь можно загрузить данные, если нужно
        }
    }
}
