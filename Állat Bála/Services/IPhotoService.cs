namespace PetManager.Services;

public interface IPhotoService
{
    Task<string?> TakePhotoAsync();
    Task<string?> PickPhotoAsync();
    string GetFullPath(string fileName);
    void Delete(string? fileName);
}