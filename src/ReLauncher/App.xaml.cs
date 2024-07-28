using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Relauncher.Core.Loggers;
using Relauncher.Extensions;
using Relauncher.Views;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Relauncher;

public partial class App : System.Windows.Application
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private static readonly IHost _host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
            RuntimeHelper.CheckSingleInstance(AppConfig.PackName);
            Log.Logger = LoggerConfiguration.CreateDefault()
                .UseType(LoggerType.Async)
                .UseLevel(LogLevel.Trace)
                .WriteToFile(
                    logFolder: AppConfig.LogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @$"{AppConfig.PackName}\log"),
                    fileName: $"{AppConfig.PackName}_{DateTime.Now:yyyyMMdd}.log"
                )
                .CreateLogger();

            //ConfigurationManager.Setup(SpecialPathHelper.GetPath("config.yaml"));
            //MuiLanguage.SetupLanguage(Configurations.Language.Get());
            //DpiAwareHelper.SetProcessDpiAwareness();

            services.AddSingleton(Log.Logger);
            //services.AddSingleton<MainViewModel>();
            services.AddSingleton<MainWindow>();
        })
        .Build();

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            await _host.StartAsync();
            GetService<MainWindow>()?.Show();
            Log.Information("Welcome to reLauncher.");
        }
        catch (Exception ex)
        {
            // DEBUG only, no overhead
            Debug.WriteLine(ex);

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        await _host.StopAsync();
        _host.Dispose();
    }

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T? GetService<T>() where T : class
    {
        return _host.Services.GetService(typeof(T)) as T;
    }

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    /// <returns></returns>
    public static object? GetService(Type type)
    {
        return _host.Services.GetService(type);
    }
}
