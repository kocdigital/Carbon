using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Carbon.MassTransit;
using Platform360.Domain.Messages.SolutionCreationSaga;
using Carbon.WebApplication.SolutionService.Services;
using Carbon.WebApplication.SolutionService.Domain;
using Carbon.WebApplication.SolutionService.Consumers;
using MassTransit;

namespace Carbon.WebApplication.SolutionService
{
    public static class ISolutionServiceBuilderExtensions
    {
        /// <summary>
        /// Register your Service as Solution to TenantManagement, so that your solution will be available in product list
        /// </summary>
        /// <typeparam name="TContext">Your Database Context</typeparam>
        /// <param name="app"></param>
        public async static void RegisterAsSolution(this IApplicationBuilder app, SolutionCreationRequest solutionDetails)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var solutionRegistrationService = serviceScope.ServiceProvider.GetRequiredService<ISolutionRegistrationService>();
                await solutionRegistrationService.AddAsSolution(solutionDetails);
            }
        }

        /// <summary>
        /// Register your Service as FeatureSet to TenantManagement, so that your featureset will be available in product list related to given solution
        /// </summary>
        /// <param name="app"></param>
        public async static void RegisterAsFeatureSet(this IApplicationBuilder app, FeatureSetCreationRequest featureSetDetails)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var solutionRegistrationService = serviceScope.ServiceProvider.GetRequiredService<ISolutionRegistrationService>();
                await solutionRegistrationService.AddAsFeatureSet(featureSetDetails);
            }
        }

        /// <summary>
        /// Configure your service as solution-managed by Tenant Management, Requires MassTransit -> RabbitMQ or ServiceBus
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureAsSolutionService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISolutionRegistrationService, SolutionRegistrationService>();
            var bsp = services.BuildServiceProvider();
            var registrationConfigurator = bsp.GetService<IRegistrationConfigurator>();

            if (registrationConfigurator == null)
            {
                services.AddMassTransitBus(cfg =>
                {
                    cfg.AddConsumer<SolutionSagaCompletionFailedConsumer>();
                    cfg.AddConsumer<SolutionSagaCompletionSucceedConsumer>();
                    cfg.AddConsumer<FeatureSetSagaCompletionFailedConsumer>();
                    cfg.AddConsumer<FeatureSetSagaCompletionSucceedConsumer>();

                    cfg.AddRabbitMqBus(configuration, (provider, busFactoryConfig) =>
                    {
                        busFactoryConfig.ReceiveEndpoint("App-Solution-Fail", e => { e.Consumer<SolutionSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint("App-Solution-Succeed", e => { e.Consumer<SolutionSagaCompletionSucceedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint("App-Featureset-Fail", e => { e.Consumer<FeatureSetSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint("App-Featureset-Succeed", e => { e.Consumer<FeatureSetSagaCompletionSucceedConsumer>(provider); });

                        busFactoryConfig.Publish<ISolutionCreationRequest>(x => { });
                        busFactoryConfig.Publish<IFeatureSetCreationRequest>(x => { });

                    });

                    cfg.AddServiceBus(configuration, (provider, busFactoryConfig) =>
                    {
                        busFactoryConfig.ReceiveEndpoint("App-Solution-Fail", e => { e.Consumer<SolutionSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint("App-Solution-Succeed", e => { e.Consumer<SolutionSagaCompletionSucceedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint("App-Featureset-Fail", e => { e.Consumer<FeatureSetSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint("App-Featureset-Succeed", e => { e.Consumer<FeatureSetSagaCompletionSucceedConsumer>(provider); });

                        busFactoryConfig.Publish<ISolutionCreationRequest>(x => { });
                        busFactoryConfig.Publish<IFeatureSetCreationRequest>(x => { });
                    });

                });
            }
            else
            {
                registrationConfigurator.AddConsumer<FeatureSetSagaCompletionSucceedConsumer>();
                registrationConfigurator.AddConsumer<FeatureSetSagaCompletionFailedConsumer>();
                registrationConfigurator.AddConsumer<SolutionSagaCompletionSucceedConsumer>();
                registrationConfigurator.AddConsumer<SolutionSagaCompletionFailedConsumer>();

                var bsp2 = services.BuildServiceProvider();
                var busControl = bsp2.GetService<IBusControl>();
                busControl.ConnectReceiveEndpoint("App-Solution-Fail", (cfg) =>
                {
                    cfg.Consumer<SolutionSagaCompletionFailedConsumer>(bsp);
                });
                busControl.ConnectReceiveEndpoint("App-Solution-Succeed", (cfg) =>
                {
                    cfg.Consumer<SolutionSagaCompletionSucceedConsumer>(bsp);
                });
                busControl.ConnectReceiveEndpoint("App-Featureset-Fail", (cfg) =>
                {
                    cfg.Consumer<FeatureSetSagaCompletionFailedConsumer>(bsp);
                });
                busControl.ConnectReceiveEndpoint("App-Featureset-Succeed", (cfg) =>
                {
                    cfg.Consumer<FeatureSetSagaCompletionSucceedConsumer>(bsp);
                });
            }



        }
    }
}
