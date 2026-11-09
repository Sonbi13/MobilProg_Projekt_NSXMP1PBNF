using Microsoft.Extensions.Logging;
using FuelLog.Services;
using FuelLog.ViewModels;
using FuelLog.Views;

namespace FuelLog
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

#if DEBUG
    		builder.Logging.AddDebug();

            builder.Services.AddSingleton<IRefuelRepository, RefuelRepository>();

            builder.Services.AddTransient<RefuelsViewModel>();
            builder.Services.AddTransient<RefuelsPage>();

            
            builder.Services.AddSingleton<ILocationService, LocationService>();
            builder.Services.AddTransient<EditRefuelViewModel>();
            builder.Services.AddTransient<EditRefuelPage>();

            
            builder.Services.AddSingleton<IExportService, ExportService>();
            builder.Services.AddTransient<StatsViewModel>();
            builder.Services.AddTransient<StatsPage>();
#endif

            return builder.Build();
        }
    }
}
