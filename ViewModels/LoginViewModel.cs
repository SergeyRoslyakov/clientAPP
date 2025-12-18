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
    using System.Windows.Input;

    namespace clientAPP.ViewModels
    {
        public class LoginViewModel : BaseViewModel
        {
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
            public ICommand TestNavigationCommand { get; }

            public LoginViewModel()
            {
                System.Diagnostics.Debug.WriteLine("LoginViewModel created");
                Title = "Вход";

                // Команда для входа
                LoginCommand = new Command(async () =>
                {
                    await Shell.Current.DisplayAlert("Вход", "Авторизация прошла успешно!", "OK");

                    // Просто заменяем MainPage приложения
                    App.Current.MainPage = new Pages.MainPage();
                });

                // Отдельная команда для теста навигации
                TestNavigationCommand = new Command(async () =>
                {
                    // Этот вариант всегда работает
                    await Shell.Current.GoToAsync(nameof(Pages.MainPage));
                });
            }
        }
    }
}