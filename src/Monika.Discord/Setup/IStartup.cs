using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monika.Setup
{
    interface IStartup
    {
        void ConfigureServices(IServiceCollection builder);
        void ConfigureLogging(ILoggerFactory loggerFactory);
        Task Run(IServiceProvider services,
            CancellationToken cancellationToken);
    }
}
