using clientAPP.ViewModels;
using Microsoft.Extensions.DependencyInjection;
namespace clientAPP.Pages;

public partial class ClientsPage : ContentPage
{
    public ClientsPage(ClientsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ClientsViewModel viewModel)
        {
            viewModel.LoadDataCommand?.Execute(null);
        }
    }
}