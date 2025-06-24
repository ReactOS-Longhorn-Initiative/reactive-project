using System;
using Avalonia;

namespace ReactiveDream.Desktop
{
    class DesktopProgram
    {
        const bool _OVERLAY_POPUPS = true;

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args) 
            => BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args)
            ;

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppConstruction.BuildAppShared<App>()
                .UsePlatformDetect()
                .With(new Win32PlatformOptions()
                {
                    OverlayPopups = _OVERLAY_POPUPS
                })
                .With(new X11PlatformOptions()
                {
                    OverlayPopups = _OVERLAY_POPUPS
                })
                .With(new AvaloniaNativePlatformOptions()
                {
                    OverlayPopups = _OVERLAY_POPUPS
                })
            ;
    }
}
