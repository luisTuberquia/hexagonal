using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using Swashbuckle.AspNetCore.Filters;
using FDLM.Infrastructure.EntrypointsAdapters.Resources;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities;

namespace FDLM.Infrastructure.EntrypointsAdapters.Injections
{
    public static class EntrypointsInjections
    {
        public static IServiceCollection AddEntrypointInjections(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(opts =>
            {

            }).AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.PropertyNamingPolicy = null;
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            var assembly = Assembly.Load("FDLM.Infrastructure.EntrypointsAdapters");
            services.AddAutoMapper(assembly);
            services.AddHealthChecks();
            services.AddSwaggerInjections(configuration);

            services.AddSingleton<IInfraEntrypointsResourceService, InfraEntrypointsResourceService>();
            services.AddSingleton<IRestTools, RestTools>();

            return services;
        }        
    }
}
