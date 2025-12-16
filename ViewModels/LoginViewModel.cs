using clientAPP.Models;
using clientAPP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace clientAPP.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private string _email = string.Empty;
        private string _password = string.Empty;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Вход";

            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Ошибка", "Заполните все поля", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var result = await _apiService.LoginAsync(Email, Password);

                if (!string.IsNullOrEmpty(result.Token))
                {
                    await Shell.Current.DisplayAlert("Успех", "Вход выполнен!", "OK");
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Неверный email или пароль", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RegisterAsync()
        {
            var username = await Shell.Current.DisplayPromptAsync(
                "Регистрация",
                "Введите имя пользователя:",
                "Зарегистрировать",
                "Отмена");

            if (!string.IsNullOrWhiteSpace(username))
            {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Заполните email и пароль", "OK");
                    return;
                }

                IsBusy = true;

                try
                {
                    var result = await _apiService.RegisterAsync(username, Email, Password);

                    if (!string.IsNullOrEmpty(result.Token))
                    {
                        await Shell.Current.DisplayAlert("Успех", "Регистрация выполнена!", "OK");
                        await Shell.Current.GoToAsync("//MainPage");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Ошибка", "Ошибка регистрации", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Ошибка", ex.Message, "OK");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }
    }
}
