using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;

namespace TodoApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(), "Logs/log.json",
                                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                                rollingInterval : RollingInterval.Day)
                .WriteTo.File(new JsonFormatter(), "Logs/errors.json", restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
                .CreateLogger();
            try
            {
                Log.Information("Starting application.");
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Exception in application.");
            }
            finally
            {
                Log.Information("Exiting application.");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
