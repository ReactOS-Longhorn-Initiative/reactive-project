using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;

[assembly: SupportedOSPlatform("browser")]
namespace ReactiveDream.Browser
{
    internal sealed partial class BrowserProgram
    {
        private static Task Main(string[] args)
            => BuildAvaloniaApp()
                .StartBrowserAppAsync("out")
            ;

        public static AppBuilder BuildAvaloniaApp()
            => AppConstruction.BuildAppShared<App>(RuntimePlatformType.Browser);
    }
}