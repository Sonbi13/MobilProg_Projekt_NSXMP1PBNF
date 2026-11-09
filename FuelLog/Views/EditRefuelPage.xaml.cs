using FuelLog.ViewModels;

namespace FuelLog.Views;

public partial class EditRefuelPage : ContentPage
{
    public EditRefuelPage(EditRefuelViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}