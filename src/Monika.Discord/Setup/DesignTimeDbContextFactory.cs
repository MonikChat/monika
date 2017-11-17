using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Monika.Setup
{
    public class DesignTimeDbContextFactory<TContext>
        : IDesignTimeDbContextFactory<TContext>, IDisposable
        where TContext : DbContext
    {
        private IServiceScope _scope;

        public virtual TContext CreateDbContext(string[] args)
        {
            var application = Program.BuildApplication(args);

            var scopeFactory = application.Services
                .GetRequiredService<IServiceScopeFactory>();

            _scope = scopeFactory.CreateScope();

            return _scope.ServiceProvider.GetRequiredService<TContext>();
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}