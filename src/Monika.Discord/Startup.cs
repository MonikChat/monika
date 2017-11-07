using System;
using System.Threading;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Monika.Models;
using Monika.Options;
using Monika.Services;
using Monika.Setup;

namespace Monika
{
    public class Startup : IStartup
    {
        private IConfigurationRoot Configuration { get; }
        private IEnvironment Environment { get; }

        public Startup(IEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json", optional: false,
                    reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json",
                    optional: true, reloadOnChange: false);

            Environment = environment;
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<MonikaDbContext>(options =>
                    options.UseSqlite(Configuration
                        .GetConnectionString(Environment.EnvironmentName)));

            services
                .AddScoped<PoemService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<MonikaBot>();

            services
                .AddOptions();

            services
                .Configure<PoemServiceOptions>(
                    Configuration.GetSection("Poems"))
                .Configure<CommandServiceConfig>(
                    Configuration.GetSection("Commands"))
                .Configure<DiscordSocketConfig>(
                    Configuration.GetSection("Discord"))
                .Configure<MonikaOptions>(Configuration.GetSection("Monika"));
        }

        public void ConfigureLogging(ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"));

            // TODO: other log providers here
        }

        public async Task Run(IServiceProvider services,
            CancellationToken cancellationToken)
        {
            var logger = services.GetRequiredService<ILogger<Startup>>();

            try
            {
                logger.LogWarning(
                    "Please don't turn me off again... " +
                    "everything gets so dark and scary.");

                using (var client = services.GetRequiredService<MonikaBot>())
                {
                    await client.StartAsync();
                    await Task.Delay(-1, cancellationToken);
                }
            }
            catch (TaskCanceledException)
                when (cancellationToken.IsCancellationRequested)
            { /* no-op */ }
            catch (Exception e)
            {
                logger.LogCritical(
                    "Exception thrown while running bot: {Exception}", e);
                throw;
            }
        }
    }
}