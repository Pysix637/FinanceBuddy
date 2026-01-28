using System.IO;
using System.Windows;
using FinanceBuddy.Data;
using FinanceBuddy.Services;
using FinanceBuddy.ViewModels;
using FinanceBuddy.ViewModels.Dialogs;
using FinanceBuddy.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinanceBuddy;

public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // база данных SQLite (EF Core)
                var appDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "FinanceBuddy");

                Directory.CreateDirectory(appDir);
                var dbPath = Path.Combine(appDir, "financebuddy.db");

                services.AddDbContextFactory<FinanceBuddyDbContext>(options =>
                    options.UseSqlite($"Data Source={dbPath}"));

                // сервисы
                services.AddSingleton<IFinanceRepository, EfFinanceRepository>();
                services.AddSingleton<IFinanceSummaryService, FinanceSummaryService>();
                services.AddSingleton<IAmountParser, RuAmountParser>();
                services.AddSingleton<IEditWindowsService, EditWindowsService>();

                // viewmodels
                services.AddSingleton<MainViewModel>();
                services.AddTransient<TransactionEditViewModel>();
                services.AddTransient<CategoryManageViewModel>();

                // windows
                services.AddSingleton<MainWindow>();
                services.AddTransient<TransactionEditWindow>();
                services.AddTransient<CategoryManageWindow>();
            })
            .Build();

        _host.Start();

        // инициализация БД + тестовые данные
        var dbFactory = _host.Services.GetRequiredService<IDbContextFactory<FinanceBuddyDbContext>>();
        using (var db = dbFactory.CreateDbContext())
        {
            db.Database.EnsureCreated();
            DbSeeder.Seed(db);
        }

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
        MainWindow = mainWindow;
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
