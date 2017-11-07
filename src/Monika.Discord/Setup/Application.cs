using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monika.Setup
{
    public class Application
    {
        private Func<IServiceProvider, CancellationToken, Task> _runMethod;
        private CancellationTokenSource _tokenSource;
        private IServiceProvider _services;
        private bool _running;

        public IServiceProvider Services => _services;
        public bool Running => _running;

        internal Application(
            Func<IServiceProvider, CancellationToken, Task> runMethod,
            IServiceProvider services)
        {
            _runMethod = runMethod;
            _services = services;
        }

        public async Task RunAsync()
        {
            if (_running)
                throw new InvalidOperationException(
                    "The application is already running");

            _tokenSource = new CancellationTokenSource();

            InstallCtrlCHandler();
            Console.WriteLine(
                "Application started. Press Ctrl+C to cancel...");

            var task = _runMethod(_services, _tokenSource.Token);
            _running = true;

            try
            {
                await task.ConfigureAwait(false);
            }
            catch (TaskCanceledException) when (task.IsCanceled)
            { }

            Console.WriteLine("Application stopped.");
        }

        public void Run()
            => RunAsync().GetAwaiter().GetResult();

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private void InstallCtrlCHandler()
        {
            Console.CancelKeyPress += (s, e) =>
            {
                if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                {
                    _tokenSource.Cancel();
                    e.Cancel = true;
                }
            };
        }
    }
}