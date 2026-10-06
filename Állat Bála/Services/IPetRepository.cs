using PetManager.Models;

namespace PetManager.Services;

public interface IPetRepository
{
    Task<List<Pet>> GetAllAsync(string? search = null);
    Task<Pet?> GetAsync(int id);
    Task SaveAsync(Pet pet);   
    Task DeleteAsync(Pet pet);
}