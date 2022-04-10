using System;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps.IdentityServer.RabbitMQ
{
    public static class RabbitMqIdOpsBuilderExtensions
    {
        public static IIdOpsIdentityServerBuilder UseRabbitMq(
            this BusBuilder builder,
            RabbitMqOptions options)
        {
            builder.IdOpsBuilder.Services.AddMassTransit(s =>
            {
                builder.BusSetup?.Invoke(s);

                s.UsingRabbitMq((provider, cfg) =>
                {
                    cfg.Host(options.Host, c =>
                    {
                        c.Username(options.Username);
                        c.Password(options.Password);
                    });

                    cfg.ReceiveEndpoint($"id-" +
                        $"{builder.IdOpsBuilder.Options!.ServerGroup.ToLower()}-" +
                        $"{builder.IdOpsBuilder.Options!.EnvironmentName.ToLower()}",
                        e =>
                        {
                            e.ConfigureConsumers(provider);
                            e.PrefetchCount = 10;
                        });
                });
            });

            return builder.IdOpsBuilder;
        }

        public static IIdOpsIdentityServerBuilder UseRabbitMq(
            this BusBuilder builder,
            Action<RabbitMqOptions> setupAction)
        {
            var options = new RabbitMqOptions();
            setupAction.Invoke(options);

            return builder.UseRabbitMq(options);
        }

        public static IIdOpsIdentityServerBuilder UseRabbitMq(
            this BusBuilder builder)
        {
            RabbitMqOptions? options = builder.IdOpsBuilder.Configuration?
                .GetSection($"{Wellknown.ConfigSections.Messaging}:RabbitMq")
                .Get<RabbitMqOptions>();

            if (options == null)
            {
                throw new ApplicationException(
                    "Could not get RabbitMQ configuration from " +
                   $"{Wellknown.ConfigSections.Messaging}:RabbitMq." +
                    "Please check your configuration");
            }

            return builder.UseRabbitMq(options);
        }
    }
}
