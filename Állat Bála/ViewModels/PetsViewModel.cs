using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PetManager.Models;
using PetManager.Services;

namespace PetManager.ViewModels;

public partial class PetsViewModel : BaseViewModel
{
    private readonly IPetRepository _repository;

    public PetsViewModel(IPetRepository repository)
    {
        _repository = repository;
        Title = "Kedvenceim";
    }

    public ObservableCollection<Pet> Pets { get; } = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    partial void OnSearchTextChanged(string value) => _ = LoadAsync();

    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            var items = await _repository.GetAllAsync(SearchText);
            Pets.Clear();
            foreach (var item in items)
                Pets.Add(item);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A betöltés nem sikerült: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AddAsync() => Shell.Current.GoToAsync(Routes.Edit);

    [RelayCommand]
    private Task EditAsync(Pet pet) => Shell.Current.GoToAsync($"{Routes.Edit}?id={pet.Id}");

    [RelayCommand]
    private Task OpenAsync(Pet pet) => Shell.Current.GoToAsync($"{Routes.Detail}?id={pet.Id}");

    [RelayCommand]
    private async Task DeleteAsync(Pet pet)
    {
        var ok = await Shell.Current.DisplayAlert("Törlés", $"Biztosan törlöd: {pet.Name}?", "Törlés", "Mégse");
        if (!ok) return;

        try
        {
            await _repository.DeleteAsync(pet);
            Pets.Remove(pet);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A törlés nem sikerült: {ex.Message}");
        }
    }
}