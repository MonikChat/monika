using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

namespace Monika.Setup
{
    class ApplicationBuilder
    {
        private ConstructorInfo _startupConstructor;

        public Type StartupType { get; private set; }
        public IEnvironment Environment { get; private set; }

        public ApplicationBuilder()
        { }

        public ApplicationBuilder WithStartup<TStartup>()
            where TStartup : IStartup
        {
            var newStartupType = typeof(TStartup);

            var typeInfo = newStartupType.GetTypeInfo();
            var constructors = typeInfo.DeclaredConstructors
                .Where(x => !x.IsStatic)
                .ToArray();

            if (constructors.Length == 0)
                throw new InvalidOperationException(
                    "No constructor exists " +
                    $"for '{newStartupType.FullName}'");
            if (constructors.Length > 1)
                throw new InvalidOperationException(
                    "Multiple constructors exist " +
                    $"for '{newStartupType.FullName}");

            _startupConstructor = constructors[0];
            StartupType = newStartupType;
            
            return this;
        }

        public ApplicationBuilder WithEnvironment(IEnvironment environment)
        {
            Environment = environment;
            return this;
        }

        public Application Build()
        {
            var startup = CreateStartup();

            var loggerFactory = new LoggerFactory();
            startup.ConfigureLogging(loggerFactory);

            var services = new ServiceCollection();

            services.TryAdd(ServiceDescriptor
                .Singleton<ILoggerFactory, LoggerFactory>(
                    (_) => loggerFactory));
            services.TryAdd(ServiceDescriptor
                .Singleton(typeof(ILogger<>), typeof(Logger<>)));
            services.TryAdd(ServiceDescriptor.Singleton(Environment));

            startup.ConfigureServices(services);

            return new Application(startup.Run,
                services.BuildServiceProvider(true));
        }


        private IStartup CreateStartup()
        {
            var parameters = _startupConstructor.GetParameters();
            object[] constructorArguments = new object[parameters.Length];

            for(var i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;

                if (paramType == typeof(IEnvironment))
                {
                    constructorArguments[i] = Environment;
                }
                else
                {
                    throw new InvalidOperationException("Cannot create an " +
                        $"'{StartupType.FullName}' as it requires a " +
                        $"parameter of type '{paramType.FullName}' which " +
                        "cannot be satisfied.");
                }
            }

            return _startupConstructor.Invoke(constructorArguments)
                as IStartup;
        }
    }
}
