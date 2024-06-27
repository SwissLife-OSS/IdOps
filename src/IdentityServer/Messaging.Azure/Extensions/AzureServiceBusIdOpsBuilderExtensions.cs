using System;
using Azure.Identity;
using IdOps.IdentityServer.Abstractions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static IdOps.IdentityServer.Wellknown.ConfigSections;

namespace IdOps.IdentityServer.Azure
{
    public static class AzureServiceBusIdOpsBuilderExtensions
    {
        public static IIdOpsIdentityServerBuilder UseAzure(this BusBuilder builder)
        {
            AzureOptions? options = builder
                .IdOpsBuilder.Configuration?.GetSection($"{Messaging}:Azure")
                .Get<AzureOptions>();

            if (options == null)
            {
                throw new ApplicationException(
                    "Could not get AzureOptions configuration from "
                        + $"{Messaging}:Azure."
                        + "Please check you configuration");
            }

            return builder.UseAzure(options);
        }

        private static IIdOpsIdentityServerBuilder UseAzure(
            this BusBuilder builder,
            AzureOptions options)
        {
            if (options.EventHub is not null)
            {
                builder.IdOpsBuilder.Services.AddSingleton<IEventSenderWorker, EventHubSender>();
            }

            builder.IdOpsBuilder.Services.AddMassTransit(s =>
            {
                builder.BusSetup?.Invoke(s);

                if (options.ServiceBus is { })
                {
                    s.RegisterServiceBus(options.ServiceBus, builder);
                }

                if (options.EventHub is { } eventHub)
                {
                    s.RegisterEventHub(eventHub);
                }
            });

            return builder.IdOpsBuilder;
        }

        private static void RegisterEventHub(
            this IBusRegistrationConfigurator configurator,
            EventHubOptions eventHub)
        {
            configurator.AddRider(x =>
                x.UsingEventHub((_, k) =>
                {
                    if (eventHub.Namespace is { } @namespace)
                    {
                        k.Host(@namespace, new DefaultAzureCredential());
                    }
                    else if (eventHub.ConnectionString is not null)
                    {
                        k.Host(eventHub.ConnectionString);
                    }
                    else
                    {
                        throw new ApplicationException(
                            "EventHub configuration is missing. Please check your settings.");
                    }
                })
            );
        }

        private static void RegisterServiceBus(
            this IBusRegistrationConfigurator configurator,
            AzureServiceBusOptions options,
            BusBuilder builder)
        {
            configurator.UsingAzureServiceBus((provider, cfg) =>
            {
                var serverGroup = builder.IdOpsBuilder.Options!.ServerGroup.ToLower();
                var environmentName = builder.IdOpsBuilder.Options!.EnvironmentName.ToLower();
                cfg.Host(options.ConnectionString);
                cfg.ReceiveEndpoint(
                    $"id-{serverGroup}-{environmentName}",
                    e =>
                    {
                        e.ConfigureConsumers(provider);
                        e.PrefetchCount = options.PrefetchCount;
                    });
            });
        }
    }
}
