namespace FuelLog.Services;

public interface ILocationService
{
    Task<Location?> GetCurrentAsync();
}