using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using OfflineGpsApp.CodeBase.App.Adapters.GPSServiceAdapter;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace OfflineGpsApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp(oIServiceProvider => new App(oIServiceProvider))//added for GPS service - provider уже является System.IServiceProvider
                .UseMauiApp<App>()
                .UseSkiaSharp()//added for SkiaSharp
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Регистрируем MainPage в DI-контейнере
            builder.Services.AddTransient<MainPage>();//added for GPS service

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID
            builder.Services.AddSingleton<IGpsServiceAdapter, OfflineGpsApp.Platforms.Android.GpsServiceAdapterAndroid>();//added for GPS service
#else
            // Заглушка для других платформ
            builder.Services.AddSingleton<IGpsServiceAdapter>(provider => throw new System.NotSupportedException("GPS service is only supported on Android."));//added for GPS service
#endif

            return builder.Build();
        }
    }
}
