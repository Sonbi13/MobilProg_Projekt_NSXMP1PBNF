using System.Globalization;
using System.Text;
using FuelLog.Models;

namespace FuelLog.Services;

public class ExportService : IExportService
{
    public async Task ExportAndShareAsync(IEnumerable<Refuel> items)
    {
        var inv = CultureInfo.InvariantCulture;
        var sb = new StringBuilder();
        sb.AppendLine("Datum,Kmora,Liter,Ar_Ft_l,Osszeg_Ft,Teletankolas,Fogyasztas_l_100km,Benzinkut");

        foreach (var r in items)
        {
            sb.AppendLine(string.Join(",",
                r.Date.ToString("yyyy-MM-dd"),
                r.Odometer,
                r.Liters.ToString("0.##", inv),
                r.PricePerLiter.ToString("0.##", inv),
                r.TotalCost.ToString("0", inv),
                r.IsFullTank ? "igen" : "nem",
                r.Consumption?.ToString("0.0", inv) ?? "",
                Escape(r.Station)));
        }

        var path = Path.Combine(FileSystem.CacheDirectory, $"tankolasok_{DateTime.Now:yyyyMMdd_HHmm}.csv");
        await File.WriteAllTextAsync(path, sb.ToString(), new UTF8Encoding(true));

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Tankolások exportálása",
            File = new ShareFile(path)
        });
    }

    private static string Escape(string text)
        => text.Contains(',') || text.Contains('"') ? $"\"{text.Replace("\"", "\"\"")}\"" : text;
}