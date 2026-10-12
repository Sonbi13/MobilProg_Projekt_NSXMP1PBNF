using CommunityToolkit.Mvvm.ComponentModel;

namespace PetManager.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    protected static Task ShowErrorAsync(string message)
        => Shell.Current.DisplayAlert("Hiba", message , "OK");
}
