using System;
using System.Collections.Generic;
using Azure.Identity;
using IdOps.Configuration;
using IdOps.Consumers;
using IdOps.IdentityServer.Hashing;
using IdOps.Model;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class IdOpsServerBuilderExtensions
    {
        public static IIdOpsServerBuilder AddIdOpsServer(
            this IServiceCollection services,
            IConfiguration configuration,
            string configSectionName = "IdOps")
        {
            IConfigurationSection section = configuration.GetSection(configSectionName);
            var builder = new IdOpsServerBuilder(section, services);

            IdOpsServerOptions? options = section.Get<IdOpsServerOptions>();

            options.MutedClients = options.MutedClients ?? new HashSet<string>();

            options = options ?? throw new ApplicationException(
                $"Configuration not found in path: '{configSectionName}' " +
                $"please check your settings.");

            builder.Services.AddCoreServices(options, configuration);
            builder.Services.AddMessaging(options.Messaging);
            services.AddEncryptionProvider<NoEncryptionProvider>(isDefault: true);
            return builder;
        }

        private static IServiceCollection AddCoreServices(
            this IServiceCollection services,
            IdOpsServerOptions options,
            IConfiguration configuration)
        {
            services.AddSingleton(options);
            services.AddSingleton<ISharedSecretGenerator, DefaultSharedSecretGenerator>();
            services.AddSingleton<ISecretService, SecretService>();
            services.AddSingleton<IIdentityServerEventMapper>(
                _ => new IdentityServerEventMapper(options.MutedClients));

            services.AddResourceManagement();
            services.AddPublishing();
            services.AddHashing();

            services.AddTenants();
            services.AddApiResources();
            services.AddClientTemplates();
            services.AddIdentityServers();
            services.AddEnvironments();
            services.AddClients();
            services.AddUserClaimRules();
            services.AddIdentityResources();
            services.AddPersonalAccessTokens();
            services.AddApiScopes();
            services.AddGrantTypes();
            services.AddApplications();

            return services;
        }

        private static void AddResourceManagement(this IServiceCollection services)
        {
            services.AddSingleton<IResourceServiceResolver, ResourceServiceResolver>();
            services.AddSingleton<IApprovalService, ApprovalService>();
            services.AddSingleton<IResourceStores>(p => new ResourceStores(p));
            services.AddSingleton<IResourceAuthoring>(p => new ResourceAuthoring(p));
            services.AddSingleton<IResourceManager, ResourceManager>();
        }

        private static void AddPublishing(this IServiceCollection services)
        {
            services.AddSingleton<IPublishingService, PublishingService>();
            services.AddSingleton<IResourcePublisher, ResourcePublisher>();
            services.AddSingleton<IDependencyService, DependencyService>();
            services.AddSingleton<IPublishingContextFactory, InMemoryPublishingContextFactory>();
            services
                .AddSingleton<IPublishedResourceDependencyResolver,
                    PublishedResourceDependencyResolver>();
            services
                .AddSingleton<IResourceMessageFactoryResolver, ResourceMessageFactoryResolver>();
        }

        private static void AddHashing(this IServiceCollection services)
        {
            services.AddSingleton<IHashAlgorithmResolver, HashAlgorithmResolver>();
            services.AddSingleton<IPasswordProvider, PasswordProvider>();
            services.RegisterHashAlgorithms();
        }

        public static void AddKeyVaultEncryptionProvider(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAzureKeyVault(configuration);
            services.AddEncryptionProvider<KeyVaultEncryptionProvider>(isDefault: true);
        }

        private static void AddConsumers(this IBusRegistrationConfigurator busConfigurator)
        {
            busConfigurator.AddConsumer<ResourcePublishedSuccessConsumer>();
            busConfigurator.AddConsumer<ResourcePublishedErrorConsumer>();
            busConfigurator.AddConsumer<UiConsoleConsumer>();
            busConfigurator.AddConsumer<IdentityServerEventBatchConsumer>();
        }

        private static IBusRegistrationConfigurator UseRabbitMq(
            this IBusRegistrationConfigurator s,
            MessagingOptions options)
        {
            s.UsingRabbitMq((provider, cfg) =>
            {
                cfg.Host(options.Host,
                    c =>
                    {
                        c.Username(options.Username);
                        c.Password(options.Password);
                    });

                cfg.ConfigureEndpoint(provider, options);
            });

            return s;
        }

        public static void ConfigureEndpoint(
            this IBusFactoryConfigurator cfg,
            IBusRegistrationContext provider,
            MessagingOptions options)
        {
            cfg.ReceiveEndpoint(options.ReceiverQueueName,
                e =>
                {
                    e.ConfigureConsumers(provider);
                    e.PrefetchCount = 100;
                    e.Batch<IdentityServerEvent>(b =>
                    {
                        b.MessageLimit = 50;
                        b.TimeLimit = TimeSpan.FromSeconds(5);
                    });
                });
        }

        private static IBusRegistrationConfigurator UseAzureServiceBus(
            this IBusRegistrationConfigurator s,
            MessagingOptions options)
        {
            s.UsingAzureServiceBus((provider, cfg) =>
            {
                cfg.Host(options.Host);
                cfg.ConfigureEndpoint(provider, options);
            });

            return s;
        }

        private static void UseEventHub(
            this IBusRegistrationConfigurator s,
            EventHubOptions options)
        {
            s.AddRider(x =>
            {
                x.AddConsumer<IdentityServerEventBatchConsumer>();
                x.UsingEventHub((contex, k) =>
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

                    k.ReceiveEndpoint("identity-events", e => e.ConfigureConsumers(contex));

                    if (options.Storage is { } storageOption)
                    {
                        if (storageOption.Url is { } url)
                        {
                            k.Storage(new Uri(url), new DefaultAzureCredential());
                        }
                        else if (storageOption.ConnectionString is { } connectionString)
                        {
                            k.Storage(connectionString);
                        }
                        else
                        {
                            throw new ApplicationException(
                                "EventHub storage configuration is missing. Please check your settings.");
                        }
                    }
                });
            });
        }

        private static IBusRegistrationConfigurator UseInMemory(
            this IBusRegistrationConfigurator s,
            MessagingOptions options)
        {
            s.UsingInMemory((provider, cfg) =>
            {
                cfg.ConfigureEndpoint(provider, options);
            });

            return s;
        }

        private static IServiceCollection AddMessaging(
            this IServiceCollection services,
            MessagingOptions options)
        {
            services.AddMassTransit(s =>
            {
                s.AddConsumers();
                switch (options.Transport)
                {
                    case MessagingTransport.RabbitMq:
                        s.UseRabbitMq(options);
                        break;

                    case MessagingTransport.AzureServiceBus:
                        s.UseAzureServiceBus(options);
                        break;

                    case MessagingTransport.Memory:
                        s.UseInMemory(options);
                        break;
                }

                if (options.EventHub is { } eventHub)
                {
                    s.UseEventHub(eventHub);
                }
            });

            return services;
        }
    }
}
