using FuelLog.ViewModels;

namespace FuelLog.Views;

public partial class RefuelsPage : ContentPage
{
    private readonly RefuelsViewModel _viewModel;

    public RefuelsPage(RefuelsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}