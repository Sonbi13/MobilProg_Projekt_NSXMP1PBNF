using PetManager.Models;
using SQLite;

namespace PetManager.Services;

public class PetRepository : IPetRepository
{
    private Task<SQLiteAsyncConnection>? _init;

    private Task<SQLiteAsyncConnection> GetDbAsync() => _init ??= InitAsync();

    private static async Task<SQLiteAsyncConnection> InitAsync()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "pets.db3");
        var db = new SQLiteAsyncConnection(path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await db.CreateTableAsync<Pet>();
        return db;
    }

    public async Task<List<Pet>> GetAllAsync(string? search = null)
    {
        var db = await GetDbAsync();
        var query = db.Table<Pet>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(s) || p.Breed.ToLower().Contains(s));
        }

        return await query.OrderBy(p => p.Name).ToListAsync();
    }

    public async Task<Pet?> GetAsync(int id)
    {
        var db = await GetDbAsync();
        return await db.Table<Pet>().Where(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task SaveAsync(Pet pet)
    {
        var db = await GetDbAsync();
        if (pet.Id == 0) await db.InsertAsync(pet);
        else await db.UpdateAsync(pet);
    }

    public async Task DeleteAsync(Pet pet)
    {
        var db = await GetDbAsync();
        await db.DeleteAsync(pet);
    }
}