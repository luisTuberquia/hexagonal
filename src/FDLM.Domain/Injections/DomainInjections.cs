using FDLM.Domain.Resources;
using FDLM.Domain.Services;
using FDLM.Utilities;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FDLM.Domain.Injections
{
    public static class DomainInjections
    {
        public static IServiceCollection AddDomainInjections(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationAssembly = Assembly.Load("FDLM.Domain");
            services.AddValidatorsFromAssembly(applicationAssembly);

            services.AddSingleton<IDataValidator, DataValidatorService>();
            services.AddSingleton<IDomainResourceService, DomainResourceService>();            
            services.AddSingleton<IntegerOperationService>();
            services.AddSingleton<ComplexOperationService>();
            services.AddSingleton<Func<string, IOperation>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "IntegerOperationService":
                        return serviceProvider.GetService<IntegerOperationService>();
                    case "ComplexOperationService":
                        return serviceProvider.GetService<ComplexOperationService>();
                    default:
                        throw new KeyNotFoundException($"Error intentando inyectar el servicio IOperation, la key: {key} no existe.");
                }
            });

            return services;
        }
    }
}
