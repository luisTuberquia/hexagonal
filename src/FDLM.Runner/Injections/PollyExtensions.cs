using FDLM.Infrastructure.EntrypointsAdapters.Rest.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace FDLM.Runner.Injections
{
    public static class PollyExtensions
    {        
        public static IServiceCollection AddPollyInjections(this IServiceCollection services, IConfiguration configuration)
        {                        
            int? eventsAllowedBeforeBreaking = configuration.GetValue<int>("CircuitBreaker:EventsAllowedBeforeBreaking");
            int? durationOfBreakInSeconds = configuration.GetValue<int>("CircuitBreaker:DurationOfBreakInSeconds");

            ILogger<PollyLogger> logger = services.BuildServiceProvider().GetService<ILogger<PollyLogger>>();

            var circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: eventsAllowedBeforeBreaking ?? 3,
                durationOfBreak: TimeSpan.FromSeconds(durationOfBreakInSeconds ?? 3),
                onBreak: (exception, timespan) =>
                {
                    logger.LogError($"Circuit broken! Break duration: {timespan.TotalSeconds} seconds.");
                },
                onReset: () =>
                {
                    logger.LogWarning("Circuit reset!");
                },
                onHalfOpen: () =>
                {
                    logger.LogWarning("Circuit in half-open state, next call is a trial.");
                });

            services.AddSingleton<IAsyncPolicy>(circuitBreakerPolicy);
            return services;
        }
    }

    public class PollyLogger()
    {

    }
}
