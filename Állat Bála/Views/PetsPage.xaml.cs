using PetManager.ViewModels;

namespace PetManager.Views;

public partial class PetsPage : ContentPage
{
    private readonly PetsViewModel _viewModel;

    public PetsPage(PetsViewModel viewModel)
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