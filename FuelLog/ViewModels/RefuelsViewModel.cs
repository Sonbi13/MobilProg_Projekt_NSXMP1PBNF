using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuelLog.Models;
using FuelLog.Services;

namespace FuelLog.ViewModels;

public partial class RefuelsViewModel : BaseViewModel
{
    private readonly IRefuelRepository _repository;

    public RefuelsViewModel(IRefuelRepository repository)
    {
        _repository = repository;
        Title = "Tankolásnapló";
    }

    public ObservableCollection<Refuel> Refuels { get; } = new();

    [ObservableProperty]
    private string summaryText = "Még nincs rögzített tankolás";

    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            var items = await _repository.GetAllAsync();   
            var summary = FuelStats.Calculate(items);       

            Refuels.Clear();
            foreach (var item in Enumerable.Reverse(items)) 
                Refuels.Add(item);

            SummaryText = summary.Count == 0
                ? "Még nincs rögzített tankolás"
                : summary.AvgConsumption is null
                    ? $"{summary.Count} tankolás – az átlagfogyasztáshoz legalább két teletankolás kell"
                    : $"Átlagfogyasztás: {summary.AvgConsumption:0.0} l/100 km · {summary.Count} tankolás";
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
    private Task EditAsync(Refuel refuel) => Shell.Current.GoToAsync($"{Routes.Edit}?id={refuel.Id}");

    [RelayCommand]
    private Task StatsAsync() => Shell.Current.GoToAsync(Routes.Stats);

    [RelayCommand]
    private async Task DeleteAsync(Refuel refuel)
    {
        var ok = await Shell.Current.DisplayAlert("Törlés", $"Törlöd ezt a tankolást? ({refuel.Title})", "Törlés", "Mégse");
        if (!ok) return;

        try
        {
            await _repository.DeleteAsync(refuel);
            await LoadAsync(); 
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A törlés nem sikerült: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task OpenMapAsync(Refuel refuel)
    {
        if (!refuel.HasLocation) return;

        try
        {
            await Map.Default.OpenAsync(refuel.Latitude!.Value, refuel.Longitude!.Value,
                new MapLaunchOptions { Name = string.IsNullOrWhiteSpace(refuel.Station) ? "Tankolás" : refuel.Station });
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A térkép megnyitása nem sikerült: {ex.Message}");
        }
    }
}