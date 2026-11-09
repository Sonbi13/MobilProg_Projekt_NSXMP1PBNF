using FuelLog.Models;

namespace FuelLog.Services;

public interface IRefuelRepository
{
    Task<List<Refuel>> GetAllAsync();
    Task<Refuel?> GetAsync(int id);
    Task SaveAsync(Refuel refuel);   
    Task DeleteAsync(Refuel refuel);
}