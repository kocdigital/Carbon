using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Carbon.MassTransit;
using Platform360.Domain.Messages.SolutionCreationSaga;
using Carbon.WebApplication.SolutionService.Services;
using Carbon.WebApplication.SolutionService.Domain;
using Carbon.WebApplication.SolutionService.Consumers;
using MassTransit;
using System;
using MassTransit.RabbitMqTransport;
using System.Linq;

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
                var requestedObjects = serviceScope.ServiceProvider.GetRequiredService<RequestedObjects>();
                requestedObjects.Solutions.Add(solutionDetails.Solution.SolutionId);
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
                var requestedObjects = serviceScope.ServiceProvider.GetRequiredService<RequestedObjects>();
                requestedObjects.FeatureSets.Add(featureSetDetails.FeatureSet.FeatureSetId);
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
            var apiname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            RequestedObjects requestedObjects = new RequestedObjects();
            requestedObjects.FeatureSets = new System.Collections.Generic.List<Guid>();
            requestedObjects.Solutions = new System.Collections.Generic.List<Guid>();

            services.AddSingleton(requestedObjects);
            services.AddScoped<FeatureSetSagaCompletionSucceedConsumer>();
            services.AddScoped<FeatureSetSagaCompletionFailedConsumer>();
            services.AddScoped<SolutionSagaCompletionSucceedConsumer>();
            services.AddScoped<SolutionSagaCompletionFailedConsumer>();

            var bsp = services.BuildServiceProvider();
            var busControl = bsp.GetService<IBusControl>();

            if (busControl == null)
            {
                services.AddMassTransitBus(cfg =>
                {
                    cfg.AddConsumer<SolutionSagaCompletionFailedConsumer>();
                    cfg.AddConsumer<SolutionSagaCompletionSucceedConsumer>();
                    cfg.AddConsumer<FeatureSetSagaCompletionFailedConsumer>();
                    cfg.AddConsumer<FeatureSetSagaCompletionSucceedConsumer>();

                    cfg.AddRabbitMqBus(configuration, (provider, busFactoryConfig) =>
                    {
                        busFactoryConfig.ReceiveEndpoint($"App-Solution-Fail-{apiname}", e => { e.Consumer<SolutionSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint($"App-Solution-Succeed-{apiname}", e => { e.Consumer<SolutionSagaCompletionSucceedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint($"App-Featureset-Fail-{apiname}", e => { e.Consumer<FeatureSetSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint($"App-Featureset-Succeed-{apiname}", e => { e.Consumer<FeatureSetSagaCompletionSucceedConsumer>(provider); });

                        busFactoryConfig.Publish<ISolutionCreationRequest>(x => { });
                        busFactoryConfig.Publish<IFeatureSetCreationRequest>(x => { });

                    });

                    cfg.AddServiceBus(configuration, (provider, busFactoryConfig) =>
                    {
                        busFactoryConfig.ReceiveEndpoint($"App-Solution-Fail-{apiname}", e => { e.Consumer<SolutionSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint($"App-Solution-Succeed-{apiname}", e => { e.Consumer<SolutionSagaCompletionSucceedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint($"App-Featureset-Fail-{apiname}", e => { e.Consumer<FeatureSetSagaCompletionFailedConsumer>(provider); });
                        busFactoryConfig.ReceiveEndpoint($"App-Featureset-Succeed-{apiname}", e => { e.Consumer<FeatureSetSagaCompletionSucceedConsumer>(provider); });

                        busFactoryConfig.Publish<ISolutionCreationRequest>(x => { });
                        busFactoryConfig.Publish<IFeatureSetCreationRequest>(x => { });
                    });

                });
            }
            else
            {
                //registrationConfigurator.AddConsumer<FeatureSetSagaCompletionSucceedConsumer>();
                //registrationConfigurator.AddConsumer<FeatureSetSagaCompletionFailedConsumer>();
                //registrationConfigurator.AddConsumer<SolutionSagaCompletionSucceedConsumer>();
                //registrationConfigurator.AddConsumer<SolutionSagaCompletionFailedConsumer>();

                busControl.ConnectReceiveEndpoint($"App-Solution-Fail-{apiname}", (cfg) =>
                {
                    cfg.Consumer<SolutionSagaCompletionFailedConsumer>(bsp);
                });
                busControl.ConnectReceiveEndpoint($"App-Solution-Succeed-{apiname}", (cfg) =>
                {
                    cfg.Consumer<SolutionSagaCompletionSucceedConsumer>(bsp);
                });
                busControl.ConnectReceiveEndpoint($"App-Featureset-Fail-{apiname}", (cfg) =>
                {
                    cfg.Consumer<FeatureSetSagaCompletionFailedConsumer>(bsp);
                });
                busControl.ConnectReceiveEndpoint($"App-Featureset-Succeed-{apiname}", (cfg) =>
                {
                    cfg.Consumer<FeatureSetSagaCompletionSucceedConsumer>(bsp);
                });
            }

            services.AddScoped<ISolutionRegistrationService, SolutionRegistrationService>();



        }


        /// <summary>
        /// Subscribes to FeatureSet enablement for any tenant, so that any logic can be run, Requires MassTransit -> RabbitMQ or ServiceBus
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="featureSetId"></param>
        public static void SubscribeToFeatureSetForAnyTenant<T>(this IServiceCollection services, IConfiguration configuration, Guid featureSetId)
            where T : class, IConsumer
        {
            var apiname = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;


            var bsp = services.BuildServiceProvider();
            var busControl = bsp.GetService<IBusControl>();

            if (busControl == null)
            {
                services.AddMassTransitBus(cfg =>
                {
                    cfg.AddConsumer<T>();

                    cfg.AddRabbitMqBus(configuration, (provider, busFactoryConfig) =>
                    {
                        busFactoryConfig.ReceiveEndpoint($"{apiname}-featureset-notification-{featureSetId}", e =>
                        {
                            e.Consumer<T>(provider);
                            e.Bind($"featureset-notification-{featureSetId}", b => { });
                        });
                    });

                    cfg.AddServiceBus(configuration, (provider, busFactoryConfig) =>
                    {
                        busFactoryConfig.ReceiveEndpoint($"featureset-notification-{featureSetId}", e =>
                        {
                            e.Consumer<T>(provider);
                        });
                    });

                });
            }
            else
            {
                busControl.ConnectReceiveEndpoint($"{apiname}-featureset-notification-{featureSetId}", (cfg) =>
                {
                    cfg.Consumer<T>(bsp);
                    if (cfg.GetType().GetInterfaces().Contains(typeof(IRabbitMqReceiveEndpointConfigurator)))
                        ((IRabbitMqReceiveEndpointConfigurator)cfg).Bind($"featureset-notification-{featureSetId}", b => { });
                });
            }

        }
    }
}
