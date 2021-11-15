using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Core.Common;
using Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository.Context;
using Repository.Seeds;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace WebApi
{
    public class Program
    {
        public async Task Main(string[] args)
        {
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

                if (!Directory.Exists(pathBuilt))
                {
                    Directory.CreateDirectory(pathBuilt);
                }

                await applicationDbContext.Database.MigrateAsync();
                Log.Information("migrate application database");

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File(pathBuilt + "\\log.txt", rollingInterval: RollingInterval.Day, fileSizeLimitBytes: 5242880, rollOnFileSizeLimit: true)
                    .WriteTo.MSSqlServer(config.GetConnectionString("DefaultConnection"),
                        new MSSqlServerSinkOptions
                        {
                            TableName = "Logs",
                            SchemaName = "Berger",
                            AutoCreateSqlTable = true
                        })
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    // .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                    .CreateLogger();

                await DefaultRoles.SeedAsync(userManager, roleManager);
                await DefaultAdminUser.SeedAsync(userManager, roleManager);
                Log.Information("Finished Seeding Default Data");
                Log.Information("Application Starting");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(services =>
                    services.AddHostedService<BackgroundProcessor>());
    }
}
