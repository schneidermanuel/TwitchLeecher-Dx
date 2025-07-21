using Avalonia;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Avalonia.ReactiveUI;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using TwitchLeecher;

namespace AvaloniaApplication2;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
            try {
                LoadCultureLinux();
            }
            catch (Exception e) {
                Console.Error.WriteLine($"Could not selectively load user culture: {e}");
            }
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current.Register<FontAwesomeIconProvider>();
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseReactiveUI()
            .LogToTrace();
    }

    // Set date-time format according to LC_TIME.
    // By default, CurrentCulture is set up only according to LC_MESSAGES, instead of using the fine grained locale environment variables.
    // Users may not want to change LC_MESSAGES for its broadness,
    //   and configuration tools like the KDE Plasma System Settings does not provide an option to change it either.
    // https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-globalization-cultureinfo-currentculture
    private static void LoadCultureLinux() {
        // keep original general culture to limit impact
        CultureInfo localCulture = new CultureInfo(CultureInfo.CurrentCulture.Name);

        string localTimeFormat = Environment.GetEnvironmentVariable("LC_TIME");
        if (localTimeFormat != null) {
            localCulture.DateTimeFormat = new CultureInfo(localTimeFormat.Split('.')[0]).DateTimeFormat;
        }

        string localNumberFormat = Environment.GetEnvironmentVariable("LC_NUMERIC");
        if (localNumberFormat != null) {
            localCulture.NumberFormat = new CultureInfo(localNumberFormat.Split('.')[0]).NumberFormat;
        }

        CultureInfo.DefaultThreadCurrentCulture = localCulture;
    }
}