using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CorePinCash.Data;
using CorePinCash.Factory;
using CorePinCash.Repository;
using CorePinCash.Server;
using CorePinCash.Watcher;
using CorePincode.Model;
using CorePincode.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CorePincode;

internal class Program
{
    private static ManualResetEvent quitEvent = new ManualResetEvent(initialState: false);

    private static IHost host { get; set; }

    private static async Task Main(string[] args)
    {
        host = CreateHostBuilder().Build();
        if (args.Contains("--add"))
        {
            await host.Services.GetRequiredService<DevInteraction>().Start();
            Environment.Exit(0);
        }
        else
        {
            await InitializePrefs();
            host.Services.GetRequiredService<LogWatch>();
            await host.StartAsync();
        }
        Stop();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        string connectionString = ConnectionBuilder.GetConnectionString();
        return Host.CreateDefaultBuilder().ConfigureServices(delegate (IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(delegate (DbContextOptionsBuilder options)
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);
            services.AddTransient<IPinCashRepository, PinCashRepository>();
            services.AddTransient<IPinItemRepository, PinItemRepository>();
            services.AddTransient<DevInteraction>();
            services.AddTransient<PlayerInteractionController>();
            services.AddTransient<LogWatch>();
            services.AddSingleton<ServerConnection>();
            services.AddSingleton<ItemAwardFactory>();
            services.AddSingleton<Definitions>();
            services.AddLogging(delegate (ILoggingBuilder builder)
            {
                builder.AddFilter("Microsoft", LogLevel.Warning).AddFilter("System", LogLevel.Warning).AddFilter("NToastNotify", LogLevel.Warning)
                    .AddConsole();
            });
        });
    }

    private static async Task InitializePrefs()
    {
        IPinCashRepository _pincashContext = host.Services.GetRequiredService<IPinCashRepository>();
        IPinItemRepository _pinitemContext = host.Services.GetRequiredService<IPinItemRepository>();
        bool flag = (await _pincashContext.GetAvailablePincodes()).Count <= 0;
        bool flag2 = flag;
        if (flag2)
        {
            flag2 = (await _pinitemContext.GetAvailablePincodes()).Count <= 0;
        }
        if (flag2)
        {
            LogWriter.Write("Não existem pincodes válidos no momento. Inicie a aplicação pelo console: ./CorePincode --add");
        }
    }

    private static void Stop()
    {
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs eArgs)
        {
            quitEvent.Set();
            eArgs.Cancel = true;
        };
        quitEvent.WaitOne();
    }
}
