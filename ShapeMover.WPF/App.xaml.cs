using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShapeMover.Helpers.Classes;
using ShapeMover.Helpers.Interfaces;
using ShapeMover.WPF.ViewModels;
using System.Windows;

namespace ShapeMover.WPF;

public partial class App : Application
{
    public static IHost? AppHost { get; private set; }

    public App()
    {
        AppHost = Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IRandomGenerator, RandomGenerator>();
                services.AddSingleton<CirclesViewModel>();
                services.AddSingleton(s => new MainWindow()
                {
                    DataContext = s.GetRequiredService<CirclesViewModel>()
                });
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await AppHost!.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost!.StopAsync();

        base.OnExit(e);
    }
}
