using FDLM.Utilities;
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

namespace FDLM.Utilities.Injections
{
    public static class UtilitiesInjections
    {
        public static IServiceCollection AddUtilitiesInjections(this IServiceCollection services, IConfiguration configuration)
        {            
            services.AddSingleton<ITools, Tools>();

            return services;
        }
    }
}
