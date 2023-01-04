

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TimerTriggerReceiver;

[assembly: FunctionsStartup(typeof(FunctionAppTimerTriggerReceiver.Startup))]
namespace FunctionAppTimerTriggerReceiver
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<IServerRepository, ServerRepository>();
        }
    }
}
