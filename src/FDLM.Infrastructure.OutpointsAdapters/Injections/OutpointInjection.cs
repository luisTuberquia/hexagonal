using FDLM.Utilities;
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
using FDLM.Infrastructure.OutpointsAdapters.Resources;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB;
using FDLM.Application.Ports;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB.Repositories;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Repositories;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.DynamoDB.Config;
using FDLM.Infrastructure.OutpointsAdapters.Database.NoSql.LiteDB.Config;

namespace FDLM.Infrastructure.OutpointsAdapters.Injections
{
    public static class OutpointInjection
    {
        public static IServiceCollection AddOutpointInjections(this IServiceCollection services, IConfiguration configuration, string currentEnvironment)
        {
            services.AddSingleton<IInfraOutpointsResourceService, InfraOutpointsResourceService>();
            services.AddSingleton<LiteDbContext>();
            services.AddSingleton<DynamoDbClient>();
                        
            if (currentEnvironment == "local")
            {
                string active = (string)configuration.GetValue(typeof(string), "Database:Active");
                if (string.Equals("DynamoDB", active, StringComparison.InvariantCultureIgnoreCase))
                {
                    services.AddSingleton<ICalculatorOperationRepositoryPort, DynamoCalculatorOperationRepositoryAdapter>();
                }
                else
                {
                    services.AddSingleton<ICalculatorOperationRepositoryPort, LiteCalculatorOperationRepositoryAdapter>();
                }
            }
            else
            {
                services.AddSingleton<ICalculatorOperationRepositoryPort, DynamoCalculatorOperationRepositoryAdapter>();
            }

            var assembly = Assembly.Load("FDLM.Infrastructure.OutpointsAdapters");
            services.AddAutoMapper(assembly);

            services.Configure<DynamoDbConfig>(configuration.GetSection("Database:DynamoDB"));
            services.Configure<LiteDbConfig>(configuration.GetSection("Database:LiteDB"));

            return services;
        }
    }
}
