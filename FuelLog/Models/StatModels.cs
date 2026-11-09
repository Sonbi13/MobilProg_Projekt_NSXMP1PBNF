namespace FuelLog.Models;

public record StatItem(string Label, string Value);

public record MonthStat(string Label, double Total, double BarWidth)
{
    public string TotalText => $"{Total:N0} Ft";
}