using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            await app.RunAsync();
        }

        public static Application BuildApplication(string[] args)
            => new ApplicationBuilder()
                .WithEnvironment(Environment.Development)
                .WithStartup<Startup>()
                .Build();
    }
}
