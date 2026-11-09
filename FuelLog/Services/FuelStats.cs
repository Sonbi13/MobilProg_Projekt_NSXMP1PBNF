using FuelLog.Models;

namespace FuelLog.Services;

public record FuelSummary(
    int Count, double TotalLiters, double TotalCost, int Distance,
    double? AvgConsumption, double? CostPer100Km);

public static class FuelStats
{
    public static FuelSummary Calculate(IReadOnlyList<Refuel> ordered)
    {
        double? lastFullOdometer = null;
        double litersSinceFull = 0;
        var segments = new List<double>();

        foreach (var r in ordered)
        {
            r.Consumption = null;

            
            if (lastFullOdometer is not null)
                litersSinceFull += r.Liters;

            if (!r.IsFullTank)
                continue;

            if (lastFullOdometer is not null && r.Odometer > lastFullOdometer.Value)
            {
                var distance = r.Odometer - lastFullOdometer.Value;
                r.Consumption = litersSinceFull / distance * 100;
                segments.Add(r.Consumption.Value);
            }

            lastFullOdometer = r.Odometer;
            litersSinceFull = 0;
        }

        var totalDistance = ordered.Count >= 2 ? ordered[^1].Odometer - ordered[0].Odometer : 0;

        return new FuelSummary(
            Count: ordered.Count,
            TotalLiters: ordered.Sum(r => r.Liters),
            TotalCost: ordered.Sum(r => r.TotalCost),
            Distance: totalDistance,
            AvgConsumption: segments.Count > 0 ? segments.Average() : null,
            CostPer100Km: totalDistance > 0 ? ordered.Skip(1).Sum(r => r.TotalCost) / totalDistance * 100 : null);
    }
}