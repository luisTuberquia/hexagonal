using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

namespace FDLM.Infrastructure.EntrypointsAdapters.Injections
{
    public static class SwaggerInjections
    {        
        public static IServiceCollection AddSwaggerInjections(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(swaggerConf =>
            {
                swaggerConf.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["controller"]}_{e.ActionDescriptor.RouteValues["action"]}");
                
                // To Do, retrieve information from configuration file.
                swaggerConf.SwaggerDoc("v1", new()
                {
                    Title = "Fundación delamujer",
                    Description = "Ejemplo de consumo de servicios del microservicio [coloca aqui tu información]",
                    Version = "v1",
                    Contact = new()
                    {
                        Name = "Fundación delamujer",
                        Email = "contact@fundaciondelamujer.com",
                        Url = new Uri("https://www.fundaciondelamujer.com.co")
                    }
                });

                var xmlFilename = "FDLM.Runner.xml";
                swaggerConf.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                swaggerConf.ExampleFilters();
                swaggerConf.EnableAnnotations();
            });
            var assembly = Assembly.Load("FDLM.Infrastructure.EntrypointsAdapters");
            services.AddSwaggerExamplesFromAssemblies(assembly);
            
            return services;
        }
    }
}
