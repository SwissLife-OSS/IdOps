using System;
using Azure.Identity;
using IdOps.IdentityServer.Abstractions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.AzureEventHub;

public static class EventHubIdOpsBuilderExtensions
{
    public static IIdOpsIdentityServerBuilder UseEventHub(this BusBuilder builder)
    {
        EventHubOptions? options = builder.IdOpsBuilder.Configuration?
            .GetSection($"{Wellknown.ConfigSections.Messaging}:EventHub")
            .Get<EventHubOptions>();

        if (options == null)
        {
            throw new ApplicationException(
                "Could not get EventHub configuration from " +
                $"{Wellknown.ConfigSections.Messaging}:EventHub." +
                "Please check you configuration");
        }

        return builder.UseEventHub(options);
    }

    public static IIdOpsIdentityServerBuilder UseEventHub(
        this BusBuilder builder,
        EventHubOptions options)
    {
        builder.IdOpsBuilder.Services.AddSingleton<IEventSenderWorker, EventHubSender>();
        builder.IdOpsBuilder.Services.AddMassTransit(s =>
        {
            builder.BusSetup?.Invoke(s);

            s.AddRider(x => x.UsingEventHub((_, k) =>
            {
                if (options.Namespace is { } @namespace)
                {
                    k.Host(@namespace, new DefaultAzureCredential());
                }
                else if (options.ConnectionString is not null)
                {
                    k.Host(options.ConnectionString);
                }
                else
                {
                    throw new ApplicationException(
                        "EventHub configuration is missing. Please check your settings.");
                }
            }));
        });

        return builder.IdOpsBuilder;
    }

    public static IIdOpsIdentityServerBuilder UseEventHub(
        this BusBuilder builder,
        Action<EventHubOptions> setupAction)
    {
        var options = new EventHubOptions();
        setupAction.Invoke(options);

        return builder.UseEventHub(options);
    }
}
