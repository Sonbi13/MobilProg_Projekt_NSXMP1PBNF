using Microsoft.Extensions.Logging;
using PetManager.Services;
using PetManager.ViewModels;
using PetManager.Views;

namespace PetManager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IPetRepository, PetRepository>();
            builder.Services.AddSingleton<IPhotoService, PhotoService>();

            builder.Services.AddTransient<PetsViewModel>();
            builder.Services.AddTransient<PetsPage>();
            builder.Services.AddTransient<EditPetViewModel>();
            builder.Services.AddTransient<EditPetPage>();
            builder.Services.AddTransient<PetDetailViewModel>();
            builder.Services.AddTransient<PetDetailPage>();
            builder.Logging.AddDebug();

#if DEBUG

#endif

            return builder.Build();
        }
    }
}
