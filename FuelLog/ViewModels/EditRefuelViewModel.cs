using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuelLog.Models;
using FuelLog.Services;

namespace FuelLog.ViewModels;

[QueryProperty(nameof(RefuelId), "id")]
public partial class EditRefuelViewModel : BaseViewModel
{
    private readonly IRefuelRepository _repository;
    private readonly ILocationService _locationService;

    public EditRefuelViewModel(IRefuelRepository repository, ILocationService locationService)
    {
        _repository = repository;
        _locationService = locationService;
        Title = "Új tankolás";
    }

    [ObservableProperty] private int refuelId;
    [ObservableProperty] private DateTime date = DateTime.Today;
    [ObservableProperty] private string odometerText = string.Empty;
    [ObservableProperty] private string litersText = string.Empty;
    [ObservableProperty] private string priceText = string.Empty;
    [ObservableProperty] private bool isFullTank = true;
    [ObservableProperty] private string station = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CoordinatesText))]
    private double? latitude;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CoordinatesText))]
    private double? longitude;

    public string CoordinatesText => Latitude is null || Longitude is null
        ? "Nincs helyszín megadva"
        : $"{Latitude:F5}, {Longitude:F5}";

    async partial void OnRefuelIdChanged(int value)
    {
        if (value > 0) await LoadAsync(value);
    }

    private async Task LoadAsync(int id)
    {
        try
        {
            var r = await _repository.GetAsync(id);
            if (r is null)
            {
                await ShowErrorAsync("A tankolás már nem létezik.");
                await Shell.Current.GoToAsync("..");
                return;
            }

            Title = "Tankolás szerkesztése";
            Date = r.Date;
            OdometerText = r.Odometer.ToString();
            LitersText = r.Liters.ToString("0.##");
            PriceText = r.PricePerLiter.ToString("0.##");
            IsFullTank = r.IsFullTank;
            Station = r.Station;
            Latitude = r.Latitude;
            Longitude = r.Longitude;
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A betöltés nem sikerült: {ex.Message}");
        }
    }

    private static bool TryParseNumber(string? text, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;

        var normalized = text.Trim().Replace(" ", "").Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!TryParseNumber(OdometerText, out var odometer) || odometer <= 0)
        {
            await ShowErrorAsync("A kilométeróra állása pozitív szám legyen.");
            return;
        }
        if (!TryParseNumber(LitersText, out var liters) || liters <= 0 || liters > 200)
        {
            await ShowErrorAsync("A literszám 0 és 200 közötti szám legyen.");
            return;
        }
        if (!TryParseNumber(PriceText, out var price) || price <= 0)
        {
            await ShowErrorAsync("Az egységár pozitív szám legyen (Ft/liter).");
            return;
        }
        if (Date > DateTime.Today)
        {
            await ShowErrorAsync("A dátum nem lehet a jövőben.");
            return;
        }

        try
        {
            IsBusy = true;
            var refuel = new Refuel
            {
                Id = RefuelId,
                Date = Date,
                Odometer = (int)Math.Round(odometer),
                Liters = liters,
                PricePerLiter = price,
                IsFullTank = IsFullTank,
                Station = Station.Trim(),
                Latitude = Latitude,
                Longitude = Longitude
            };

            await _repository.SaveAsync(refuel);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A mentés nem sikerült: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UseCurrentLocationAsync()
    {
        try
        {
            IsBusy = true;
            var location = await _locationService.GetCurrentAsync();
            if (location is null)
            {
                await ShowErrorAsync("A helyszín mentéséhez engedélyezd a helymeghatározást az alkalmazásnak.");
                return;
            }

            Latitude = location.Latitude;
            Longitude = location.Longitude;
        }
        catch (FeatureNotEnabledException)
        {
            await ShowErrorAsync("A helymeghatározás ki van kapcsolva az eszközön. Kapcsold be, majd próbáld újra.");
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A pozíció lekérése nem sikerült: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ClearLocation()
    {
        Latitude = null;
        Longitude = null;
    }

    [RelayCommand]
    private Task CancelAsync() => Shell.Current.GoToAsync("..");
}