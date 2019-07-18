using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;

namespace rabbithole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try {
                await new HostBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile(
                            "appsettings.default.json", optional: true);
                        config.AddJsonFile(
                            "appsettings.json", optional: true);
                        config.AddEnvironmentVariables("RABBITHOLE_");
                    })
                    .ConfigureServices( (context, options) => {
                        options.AddEntityFrameworkNpgsql().AddDbContext<EventSinkContext>(optionsBuilder => {
                            optionsBuilder.UseNpgsql(context.Configuration.GetConnectionString("Events"));
                        });
                    })
                    .ConfigureLogging((hostContext, configLogging) =>
                    {
                        configLogging.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                        configLogging.AddConsole();
                        configLogging.AddDebug();
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<RabbitEFService>();
                    })
                    .UseConsoleLifetime()
                    .Build().MigrateDatabase<EventSinkContext>()
                    .RunAsync();
            } catch (Exception ex) {
                Console.WriteLine("Caught error starting server.");
            }
        }
    }
}
