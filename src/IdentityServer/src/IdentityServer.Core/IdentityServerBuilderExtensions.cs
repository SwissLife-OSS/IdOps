using System;
using System.Threading.Channels;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdOps.IdentityServer;
using IdOps.IdentityServer.DataConnector;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.Hashing;
using IdOps.IdentityServer.ResourceUpdate;
using IdOps.IdentityServer.Services;
using IdOps.IdentityServer.Storage;
using IdOps.Messages;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public record IdOpsIdentityServerBuilder(IServiceCollection Services)
        : IIdOpsIdentityServerBuilder
    {
        public IConfiguration? Configuration { get; init; }

        public IdOpsOptions Options { get; internal set; } = new IdOpsOptions();
    }

    public record BusBuilder(
        IIdOpsIdentityServerBuilder IdOpsBuilder,
        Action<IServiceCollectionBusConfigurator>? BusSetup);

    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddIdOps(
            this IIdentityServerBuilder builder,
            Action<IIdOpsIdentityServerBuilder>? configure)
        {
            var idOpsBuilder = new IdOpsIdentityServerBuilder(builder.Services);

            return idOpsBuilder.AddIdOps(builder, configure);
        }

        public static IIdentityServerBuilder AddIdOps(
            this IIdentityServerBuilder builder,
            IConfiguration configuration,
            Action<IIdOpsIdentityServerBuilder>? configure)
        {
            IdOpsOptions? options = configuration
                .Get<IdOpsOptions>();

            var idOpsBuilder = new IdOpsIdentityServerBuilder(builder.Services)
            {
                Configuration = configuration,
                Options = options ?? new IdOpsOptions()
            };

            return idOpsBuilder.AddIdOps(builder,configure);
        }

        private static IIdentityServerBuilder AddIdOps(
            this IIdOpsIdentityServerBuilder builder,
            IIdentityServerBuilder identityServerBuilder,
            Action<IIdOpsIdentityServerBuilder>? configure)
        {
            configure?.Invoke(builder);

            var channel = Channel
                .CreateUnbounded<IdentityEventMessage>(new UnboundedChannelOptions
                {
                    SingleWriter = true,
                    SingleReader = true,
                    AllowSynchronousContinuations = false
                });

            builder.Services.AddSingleton(channel.Reader);
            builder.Services.AddSingleton(channel.Writer);
            builder.Services.AddHostedService<BusEventSender>();
            builder.Services.AddResources();
            builder.Services.AddSingleton(builder.Options);
            builder.Services.AddSingleton<IResourceUpdateHandler, ResourceUpdateHandler>();
            builder.Services.AddSingleton<IEventSink, IdOpsEventSink>();
            builder.Services.AddSingleton<IIdOpsEventSink, BusEventSink>();
            builder.Services.AddTransient<IExtensionGrantValidator, PersonalAccessTokenGrantValidator>();
            builder.Services
                .AddSingleton<IPersonalAccessTokenValidator, PersonalAccessTokenValidator>();
            builder.Services.AddSingleton<IPersonalAccessTokenSource, LocalAccessTokenSource>();
            builder.Services.RegisterHashAlgorithms();

            if (builder.Options.EnableDataConnectors)
            {
                builder.Services.AddUserDataConnectors();
            }

            identityServerBuilder.AddIdOpsStores();

            return identityServerBuilder;
        }

        public static BusBuilder AddMassTransit(this IIdOpsIdentityServerBuilder builder)
        {
            return new BusBuilder(builder, (b) =>
            {
                b.AddConsumer<UpdateResourceConsumer>();
            });
        }

        public static IIdOpsIdentityServerBuilder UseInMemory(this BusBuilder builder)
        {
            builder.IdOpsBuilder.Services.AddMassTransit(s =>
            {
                builder.BusSetup?.Invoke(s);

                s.UsingInMemory((provider, cfg) =>
                {
                    cfg.ReceiveEndpoint(
                        $"id-{builder.IdOpsBuilder.Options!.EnvironmentName.ToLower()}", e =>
                    {
                        e.ConfigureConsumers(provider);
                    });
                });
            });

            return builder.IdOpsBuilder;
        }

        public static IIdOpsIdentityServerBuilder SetEnvironment(
            this IIdOpsIdentityServerBuilder builder,
            string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                builder.Options.EnvironmentName = name;
            }

            return builder;
        }

        public static IIdOpsIdentityServerBuilder SetServerGroup(
            this IIdOpsIdentityServerBuilder builder,
            string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                builder.Options.ServerGroup = name;
            }

            return builder;
        }

        public static IIdOpsIdentityServerBuilder EnableUserDataConnectors(
            this IIdOpsIdentityServerBuilder builder)
        {
            builder.Options.EnableDataConnectors = true;

            return builder;
        }

        public static IIdOpsIdentityServerBuilder RegisterUserDataConnector<T>(
            this IIdOpsIdentityServerBuilder builder) where T : class, IUserDataConnector
        {
            builder.Services.AddSingleton<IUserDataConnector, T>();

            return builder;
        }


        private static IIdentityServerBuilder AddIdOpsStores(
            this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IManageClientStore, ClientStore>();
            builder.Services.AddSingleton<IManageResourceStore, ResourceStore>();
            builder.Services.AddSingleton<IUserClaimsRulesService, UserClaimsRulesService>();

            builder.AddInMemoryCaching();

            builder
                .AddResourceStore<ResourceStore>()
                .AddPersistedGrantStore<PersistedGrantStore>()
                .AddClientStoreCache<ClientStore>()
                .AddCorsPolicyService<CorsPolicyService>();

            return builder;
        }
    }
}
