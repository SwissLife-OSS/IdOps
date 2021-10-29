using System;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.AzureServiceBus
{
    public static class AzureServiceBusIdOpsBuilderExtensions
    {
        public static IIdOpsIdentityServerBuilder UseAzureServiceBus(this BusBuilder builder)
        {
            AzureServiceBusOptions? options = builder.IdOpsBuilder.Configuration?
                .GetSection($"{Wellknown.ConfigSections.Messaging}:AzureServiceBus")
                .Get<AzureServiceBusOptions>();

            if (options == null)
            {
                throw new ApplicationException(
                    "Could not get AzureServiceBus configuration from " +
                    $"{Wellknown.ConfigSections.Messaging}:AzureServiceBus." +
                    "Please check you configuration");
            }

            return builder.UseAzureServiceBus(options);
        }

        public static IIdOpsIdentityServerBuilder UseAzureServiceBus(
            this BusBuilder builder,
            AzureServiceBusOptions options)
        {
            builder.IdOpsBuilder.Services.AddMassTransit(s =>
            {
                builder.BusSetup?.Invoke(s);

                s.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(options.ConnectionString);
                    cfg.ReceiveEndpoint($"id-" +
                        $"{builder.IdOpsBuilder.Options!.ServerGroup.ToLower()}-" +
                        $"{builder.IdOpsBuilder.Options!.EnvironmentName.ToLower()}",
                        e =>
                        {
                            e.ConfigureConsumers(provider);
                            e.PrefetchCount = options.PrefetchCount;
                        });
                }));
            });

            return builder.IdOpsBuilder;
        }

        public static IIdOpsIdentityServerBuilder UseAzureServiceBus(
            this BusBuilder builder,
            Action<AzureServiceBusOptions> setupAction)
        {
            var options = new AzureServiceBusOptions();
            setupAction.Invoke(options);

            return builder.UseAzureServiceBus(options);
        }
    }
}
