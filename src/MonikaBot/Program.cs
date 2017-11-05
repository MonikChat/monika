using System.Threading.Tasks;
using Monika.Setup;

namespace Monika
{
    static class Program
    {
#if CSHARP_71
        public static async Task Main(string[] args)
#else
        public static void Main(string[] args) =>
            RunAsync(args).GetAwaiter().GetResult();
        private static async Task RunAsync(string[] args)
#endif
        {
            var app = new ApplicationBuilder()
                .WithEnvironment(Environment.Development)
                .WithStartup<Startup>()
                .Build();

            await app.RunAsync();
        }
    }
}
