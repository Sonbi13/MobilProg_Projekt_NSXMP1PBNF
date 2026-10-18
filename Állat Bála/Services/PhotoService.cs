namespace PetManager.Services;

public class PhotoService : IPhotoService
{
    public async Task<string?> TakePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
            throw new FeatureNotSupportedException();

        var photo = await MediaPicker.Default.CapturePhotoAsync();
        return await SaveAsync(photo);
    }

    public async Task<string?> PickPhotoAsync()
    {
        var photo = await MediaPicker.Default.PickPhotoAsync();
        return await SaveAsync(photo);
    }

    public string GetFullPath(string fileName) => Path.Combine(FileSystem.AppDataDirectory, fileName);

    public void Delete(string? fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return;
        var path = GetFullPath(fileName);
        if (File.Exists(path)) File.Delete(path);
    }

   
    private static async Task<string?> SaveAsync(FileResult? photo)
    {
        if (photo is null) return null; 

        var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(photo.FileName)}";
        var target = Path.Combine(FileSystem.AppDataDirectory, fileName);

        await using var source = await photo.OpenReadAsync();
        await using var destination = File.Create(target);
        await source.CopyToAsync(destination);

        return fileName;
    }
}