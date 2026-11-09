using SQLite;

namespace FuelLog.Models;

public class Refuel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime Date { get; set; } = DateTime.Today;
    public int Odometer { get; set; }            
    public double Liters { get; set; }
    public double PricePerLiter { get; set; }    
    public bool IsFullTank { get; set; } = true;
    public string Station { get; set; } = string.Empty;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    
    [Ignore] public double? Consumption { get; set; }   
    [Ignore] public double TotalCost => Liters * PricePerLiter;
    [Ignore] public bool HasLocation => Latitude.HasValue && Longitude.HasValue;

    [Ignore] public string Title => $"{Date:yyyy.MM.dd.} · {Odometer:N0} km";
    [Ignore] public string Details => $"{Liters:0.0} l · {PricePerLiter:0.#} Ft/l · {TotalCost:N0} Ft";
    [Ignore] public string ConsumptionText => Consumption is null ? "–" : $"{Consumption:0.0} l/100 km";
}