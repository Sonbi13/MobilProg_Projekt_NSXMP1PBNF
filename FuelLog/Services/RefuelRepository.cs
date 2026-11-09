using SQLite;
using FuelLog.Models;

namespace FuelLog.Services;

public class RefuelRepository : IRefuelRepository
{
    private Task<SQLiteAsyncConnection>? _init;

    private Task<SQLiteAsyncConnection> GetDbAsync() => _init ??= InitAsync();

    private static async Task<SQLiteAsyncConnection> InitAsync()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "fuellog.db3");
        var db = new SQLiteAsyncConnection(path,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        await db.CreateTableAsync<Refuel>();
        return db;
    }

    public async Task<List<Refuel>> GetAllAsync()
    {
        var db = await GetDbAsync();
        return await db.Table<Refuel>().OrderBy(r => r.Odometer).ToListAsync();
    }

    public async Task<Refuel?> GetAsync(int id)
    {
        var db = await GetDbAsync();
        return await db.Table<Refuel>().Where(r => r.Id == id).FirstOrDefaultAsync();
    }

    public async Task SaveAsync(Refuel refuel)
    {
        var db = await GetDbAsync();
        if (refuel.Id == 0) await db.InsertAsync(refuel);
        else await db.UpdateAsync(refuel);
    }

    public async Task DeleteAsync(Refuel refuel)
    {
        var db = await GetDbAsync();
        await db.DeleteAsync(refuel);
    }
}