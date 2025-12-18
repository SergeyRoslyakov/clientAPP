using System.Windows.Input;
using clientAPP.DTO;
using clientAPP.Services;
using Microsoft.Maui.Controls;

namespace clientAPP.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError = false;

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand TestCredentialsCommand { get; }

        public LoginViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Вход в Techno-Fix";

            LoginCommand = new Command(async () => await LoginAsync());
            RegisterCommand = new Command(async () => await RegisterAsync());
            TestCredentialsCommand = new Command(() => SetTestCredentials());

            // Автозаполнение тестовыми данными для отладки
#if DEBUG
            SetTestCredentials();
#endif
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = "";

            try
            {
                // Валидация
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Введите email";
                    HasError = true;
                    return;
                }

                if (!Email.Contains("@"))
                {
                    ErrorMessage = "Введите корректный email";
                    HasError = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Введите пароль";
                    HasError = true;
                    return;
                }

                Console.WriteLine($"Попытка входа для email: {Email}");

                var loginDto = new LoginDto
                {
                    Email = Email.Trim(),
                    Password = Password
                };

                var user = await _apiService.LoginAsync(loginDto);

                if (!string.IsNullOrEmpty(user.Token))
                {
                    Console.WriteLine($"Успешный вход. Токен получен. Роль: {user.Role}");

                    // Переходим на страницу устройств
                    if (Application.Current?.MainPage is NavigationPage navigationPage)
                    {
                        await navigationPage.Navigation.PushAsync(new Pages.DevicesPage());
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new Pages.DevicesPage());
                    }
                }
                else
                {
                    ErrorMessage = "Неверные учетные данные";
                    HasError = true;
                    Console.WriteLine("Неверные учетные данные - токен не получен");
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = httpEx.Message;
                HasError = true;
                Console.WriteLine($"HTTP ошибка: {httpEx}");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
                HasError = true;
                Console.WriteLine($"Общая ошибка: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;

            try
            {
                // Запрашиваем дополнительные данные для регистрации
                var username = await Application.Current.MainPage.DisplayPromptAsync(
                    "Регистрация", "Имя пользователя:");

                if (string.IsNullOrWhiteSpace(username))
                {
                    ErrorMessage = "Имя пользователя обязательно";
                    HasError = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Email обязателен";
                    HasError = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Пароль обязателен";
                    HasError = true;
                    return;
                }

                var registerDto = new RegisterDto
                {
                    Username = username,
                    Email = Email,
                    Password = Password,
                    Role = "Technician", // По умолчанию регистрируем как техника
                    TechnicianId = 0
                };

                var user = await _apiService.RegisterAsync(registerDto);

                if (!string.IsNullOrEmpty(user.Token))
                {
                    // Успешная регистрация и вход
                    await Application.Current.MainPage.DisplayAlert(
                        "Успех",
                        "Регистрация прошла успешно! Вы автоматически вошли в систему.",
                        "OK");

                    // Переходим на страницу устройств
                    if (Application.Current?.MainPage is NavigationPage navigationPage)
                    {
                        await navigationPage.Navigation.PushAsync(new Pages.DevicesPage());
                    }
                    else
                    {
                        Application.Current.MainPage = new NavigationPage(new Pages.DevicesPage());
                    }
                }
                else
                {
                    ErrorMessage = "Ошибка регистрации";
                    HasError = true;
                }
            }
            catch (HttpRequestException httpEx)
            {
                ErrorMessage = httpEx.Message;
                HasError = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка регистрации: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SetTestCredentials()
        {

            Email = "admin@technofix.com";
            Password = "admin123";
        }
    }
}