using Microsoft.Maui.LifecycleEvents;
using ScheduleLab.Services;
using ScheduleLab.Strategies;
using ScheduleLab.ViewModels;

namespace ScheduleLab;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        
        builder.ConfigureLifecycleEvents(events =>
        {
#if WINDOWS
            events.AddWindows(windows => windows.OnWindowCreated(window =>
            {
                window.AppWindow.Closing += async (sender, args) =>
                {
                    if (AppExit.Confirmed) return;
                    args.Cancel = true;
                    await AppExit.RequestAsync(new DialogService());
                };
            }));
#endif
        });

        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<GoogleDriveService>();
        builder.Services.AddSingleton<XmlValuesLoader>();
        builder.Services.AddSingleton<HtmlTransformer>();

        
        builder.Services.AddSingleton<ISearchStrategy, SaxSearchStrategy>();
        builder.Services.AddSingleton<ISearchStrategy, DomSearchStrategy>();
        builder.Services.AddSingleton<ISearchStrategy, LinqSearchStrategy>();

        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
