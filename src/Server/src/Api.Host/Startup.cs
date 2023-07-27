using IdOps.Abstractions;
using IdOps.Api.Security;
using IdOps.AspNet;
using IdOps.Authorization;
using IdOps.GraphQL;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdOps.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            IIdOpsServerBuilder builder = services
                .AddIdOpsServer(Configuration)
                .AddMongoStore()
                .AddGraphQLServer();

            services.AddAuthTokenGenerator();
            services.AddMemoryCache();
            services.AddAuthentication(HostEnvironment, builder.Configuration);
            services.AddAuthorization(builder.Configuration);
            services.Configure<MassTransitHostOptions>(o => o.WaitUntilStarted = true);
            services.AddSignalR();
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddUserContextAccessor();

            if (HostEnvironment.IsDevelopment())
            {
                services.AddHostedService<IdOpsSeeder>();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultForwardedHeaders();
            app.UseCookiePolicy();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdOps();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGraphQL();
                endpoints.MapControllers();
                endpoints.MapHub<OpsHub>("/signal");
                endpoints.MapHub<AccessTokenHub>("/clients/hub");
                endpoints.MapAuthorizeClient();
            });

            if (!env.IsDevelopment())
            {
                app.UseIdOpsUI();
            }
        }
    }
}
