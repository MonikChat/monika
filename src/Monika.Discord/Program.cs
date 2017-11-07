using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Monika.Models;
using Monika.Setup;

namespace Monika
{
    class Program
    {
#if CSHARP_71
        public static async Task Main(string[] args)
#else
        public static void Main(string[] args) =>
            RunAsync(args).GetAwaiter().GetResult();
        private static async Task RunAsync(string[] args)
#endif
        {
            var app = BuildApplication(args);

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services
                        .GetRequiredService<MonikaDbContext>();
                    SeedData.Seed(context);
                }
                catch (System.Exception ex)
                {
                    var logger = services
                        .GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex,
                        "An error occured while seeding the database.");
                }
            }

            await app.RunAsync();
        }

        public static Application BuildApplication(string[] args)
            => new ApplicationBuilder()
                .WithEnvironment(Environment.Development)
                .WithStartup<Startup>()
                .Build();
    }
}
