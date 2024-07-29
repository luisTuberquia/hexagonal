using FluentValidation;
using Microsoft.Extensions.Options;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Filters;
using FDLM.Runner.Injections;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using FDLM.Application.Injections;
using FDLM.Domain.Injections;
using FDLM.Infrastructure.EntrypointsAdapters.Injections;
using FDLM.Infrastructure.OutpointsAdapters.Injections;
using FDLM.Utilities.Injections;

#region Configuraciones Generales a Todos los Proyectos
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("es");

builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddMemoryCache();
builder.Services.AddPollyInjections(builder.Configuration);
#endregion

#region Propiedades de AppSettings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())    
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables();
#endregion

#region Inyección de Servicios
builder.Services.AddApplicationInjections(builder.Configuration);
builder.Services.AddDomainInjections(builder.Configuration);
builder.Services.AddEntrypointInjections(builder.Configuration);
builder.Services.AddOutpointInjections(builder.Configuration, builder.Environment.EnvironmentName);
builder.Services.AddUtilitiesInjections(builder.Configuration);
#endregion

#region Configuración de la aplicación web - pipeline HTTP
var app = builder.Build();

    var env = builder.Environment.EnvironmentName;
    if (string.Equals("local", env, StringComparison.InvariantCultureIgnoreCase) ||
        string.Equals("dev", env, StringComparison.InvariantCultureIgnoreCase) ||
        string.Equals("qa", env, StringComparison.InvariantCultureIgnoreCase)) 
    {
        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "ms-my-microservice");
            x.DefaultModelsExpandDepth(-1);
        });
    }

    app.UseRouting().UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/v1/health");
    });

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
#endregion

public partial class Program { }
