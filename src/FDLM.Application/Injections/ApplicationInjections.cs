using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
using FDLM.Application.UseCases.Calculator;
using FDLM.Application.Resources;

namespace FDLM.Application.Injections
{
    public static class ApplicationInjections
    {
        public static IServiceCollection AddApplicationInjections(this IServiceCollection services, IConfiguration configuration)
        {         
            services.AddSingleton<ICalculatorUseCase, CalculatorUseCase>();
            services.AddSingleton<IApplicationResourceService, ApplicationResourceService>();

            return services;
        }        
    }
}
