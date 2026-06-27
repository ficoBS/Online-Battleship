using Microsoft.Extensions.Logging;
using Online_Battleship.Services;
using Online_Battleship.Views;
using Plugin.Maui.Audio;

namespace Online_Battleship
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

            builder.Services.AddSingleton(AudioManager.Current);
            builder.Services.AddSingleton<IOrientationService, OrientationService>();
            builder.Services.AddTransient<ShipPlacementPage>();
            builder.Services.AddTransient<GamePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}