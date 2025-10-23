using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace ReactiveDream
{
    public static class AppConstruction
    {
        public static AppBuilder BuildAppShared<TApp>(RuntimePlatformType platformType)
            where TApp : App, new()
            => AppBuilder.Configure<TApp>()
                .LogToTrace()
                .UseReactiveUI()
            ;
    }
}
