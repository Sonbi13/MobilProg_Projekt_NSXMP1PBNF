using SQLite;

namespace PetManager.Models;

public class Pet
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [MaxLength(60), NotNull]
    public string Name { get; set; } = string.Empty;

    public string Species { get; set; } = "Kutya";
    public string Breed { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; } = DateTime.Today;
    public string Notes { get; set; } = string.Empty;

    public string? PhotoFileName { get; set; }

    [Ignore]
    public string? PhotoPath => string.IsNullOrEmpty(PhotoFileName)
        ? null
        : Path.Combine(FileSystem.AppDataDirectory, PhotoFileName);

    [Ignore]
    public string Subtitle => string.IsNullOrWhiteSpace(Breed) ? Species : $"{Species} · {Breed}";

    [Ignore]
    public string AgeText
    {
        get
        {
            var today = DateTime.Today;
            var months = (today.Year - BirthDate.Year) * 12 + today.Month - BirthDate.Month;
            if (today.Day < BirthDate.Day)
            {
                months--;
                months = Math.Max(0, months);
            } 
            
            return months < 12 ? $"{months} hónapos" : $"{months / 12} éves";
        }
    }
}