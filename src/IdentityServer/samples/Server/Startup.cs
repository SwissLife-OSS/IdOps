using System;
using IdOps.IdentityServer.Azure;
using IdOps.IdentityServer.Events;
using IdOps.IdentityServer.RabbitMQ;
using IdOps.IdentityServer.Samples.DataSeeding;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdOps.IdentityServer.Samples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IIdentityServerBuilder builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.Endpoints.EnableIntrospectionEndpoint = true;
            })
            .AddDeveloperSigningCredential(persistKey: true, filename: "sign_key.jwk")
            .AddIdOps(
                Configuration.GetSection("IdOps"),
                config =>
                {
                    config.EnableUserDataConnectors();
                    config.UseMongoStores();

                    BusBuilder busBuilder = config.AddMassTransit();

                    switch (config.Options.Messaging.Transport)
                    {
                        case MessagingTransport.Memory:
                            busBuilder.UseInMemory();
                            break;
                        case MessagingTransport.RabbitMq:
                            busBuilder.UseRabbitMq();
                            break;
                        case MessagingTransport.Azure:
                            busBuilder.UseAzure();
                            break;
                    }

                }).AddProfileService<SampleProfileService>();

            services.AddSingleton<IIdOpsEventSink, ActivityEnricherSink>();
            services.Configure<MassTransitHostOptions>(o => o.WaitUntilStarted = true);
            services.ConfigureSameSiteCookies();
            services.AddSingleton<DataSeeder>();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            DataSeeder dataSeeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //dataSeeder.SeedSampleDataAsync(default)
            //    .GetAwaiter()
            //    .GetResult();

            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIpAllowListForIdOpsClients();
            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
