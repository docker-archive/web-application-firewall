using DockerSamples.WebApplicationFirewall.App.AttackCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace DockerSamples.WebApplicationFirewall.App
{
    class Program
    {
        private static ManualResetEvent _ResetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
            serviceCollection.AddSingleton(config);
            
            var loggingConfig = config.GetSection("Logging");
            var loggerFactory = new LoggerFactory().AddConsole(loggingConfig);
            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddLogging();

            serviceCollection.AddTransient<IAttackCheck, SqlInjectionAttackCheck>();
            serviceCollection.AddSingleton<ProxyController>();
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var controller = serviceProvider.GetService<ProxyController>();
            controller.StartProxy();

            _ResetEvent.WaitOne();
            controller.Stop();
        }
    }
}
