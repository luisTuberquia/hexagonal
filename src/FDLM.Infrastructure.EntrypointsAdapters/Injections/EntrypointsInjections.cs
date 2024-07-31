using Amazon.SQS;
using FDLM.Application.Ports;
using FDLM.Infrastructure.EntrypointsAdapters.Resources;
using FDLM.Infrastructure.EntrypointsAdapters.Rest.Utilities;
using FDLM.Infrastructure.EntrypointsAdapters.SQS;
using FDLM.Infrastructure.EntrypointsAdapters.SQS.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            services.AddSingleton<AmazonSQSClient>();
            services.AddSingleton<SqsListener >();

            services.AddAutoMapper(assembly);
            services.AddHealthChecks();
            services.AddSwaggerInjections(configuration);

            services.AddSingleton<IInfraEntrypointsResourceService, InfraEntrypointsResourceService>();
            services.AddSingleton<IRestTools, RestTools>();

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();


                var jobKey = new JobKey("SqsSum");
                q.AddJob<SqsSumListenerJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{jobKey}-trigger")
                .WithCronSchedule(configuration.GetSection("CronJob:SumRequest:CronSchedule").Value ?? "0/59 * * * * ?"));
            });
            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
            services.Configure<SumRequestConfig>(configuration.GetSection("CronJob:SumRequest"));

            return services;
        }
    }
}
