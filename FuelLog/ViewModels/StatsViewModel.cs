using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FuelLog.Models;
using FuelLog.Services;

namespace FuelLog.ViewModels;

public partial class StatsViewModel : BaseViewModel
{
    private readonly IRefuelRepository _repository;
    private readonly IExportService _export;
    private List<Refuel> _current = new();

    public StatsViewModel(IRefuelRepository repository, IExportService export)
    {
        _repository = repository;
        _export = export;
        Title = "Statisztika";
    }

    public IReadOnlyList<string> Periods { get; } = new[] { "Összes", "Idei év", "Elmúlt 90 nap" };

    [ObservableProperty]
    private string selectedPeriod = "Összes";

    partial void OnSelectedPeriodChanged(string value) => _ = LoadAsync();

    public ObservableCollection<StatItem> Items { get; } = new();
    public ObservableCollection<MonthStat> Months { get; } = new();

    [RelayCommand]
    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            var all = await _repository.GetAllAsync();

            DateTime? from = SelectedPeriod switch
            {
                "Idei év" => new DateTime(DateTime.Today.Year, 1, 1),
                "Elmúlt 90 nap" => DateTime.Today.AddDays(-90),
                _ => null
            };

            _current = all.Where(r => from is null || r.Date >= from.Value).ToList();
            var s = FuelStats.Calculate(_current);

            Items.Clear();
            Items.Add(new("Tankolások száma", s.Count.ToString()));
            Items.Add(new("Megtett távolság", s.Distance > 0 ? $"{s.Distance:N0} km" : "–"));
            Items.Add(new("Összes üzemanyag", $"{s.TotalLiters:0.0} l"));
            Items.Add(new("Összköltség", $"{s.TotalCost:N0} Ft"));
            Items.Add(new("Átlagfogyasztás", s.AvgConsumption is null ? "–" : $"{s.AvgConsumption:0.0} l/100 km"));
            Items.Add(new("Költség 100 km-en", s.CostPer100Km is null ? "–" : $"{s.CostPer100Km:N0} Ft"));

            var grouped = _current
                .GroupBy(r => new DateTime(r.Date.Year, r.Date.Month, 1))
                .OrderByDescending(g => g.Key)
                .Take(6)
                .OrderBy(g => g.Key)
                .Select(g => (Month: g.Key, Total: g.Sum(r => r.TotalCost)))
                .ToList();

            var max = grouped.Count == 0 ? 1 : grouped.Max(g => g.Total);
            if (max <= 0) max = 1;

            Months.Clear();
            foreach (var g in grouped)
                Months.Add(new MonthStat($"{g.Month:yyyy. MMM}", g.Total, Math.Max(4, g.Total / max * 200)));
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"A statisztika betöltése nem sikerült: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ExportAsync()
    {
        if (_current.Count == 0)
        {
            await ShowErrorAsync("Nincs exportálható tankolás a kiválasztott időszakban.");
            return;
        }

        try
        {
            await _export.ExportAndShareAsync(_current);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Az export vagy a megosztás nem sikerült: {ex.Message}");
        }
    }
}