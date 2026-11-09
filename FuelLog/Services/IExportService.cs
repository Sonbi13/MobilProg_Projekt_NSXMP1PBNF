using FuelLog.Models;

namespace FuelLog.Services;

public interface IExportService
{
    Task ExportAndShareAsync(IEnumerable<Refuel> items);
}