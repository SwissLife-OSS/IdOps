using IdOps.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdOps
{
    public static class ClientTemplatesServiceCollectionExtensions
    {
        public static IServiceCollection AddClientTemplates(this IServiceCollection services)
        {
            services.TryAddSingleton<IClientIdGenerator, GuidClientIdGenerator>();
            
            services.AddSingleton<IClientTemplateService, ClientTemplateService>();
            services.AddSingleton<ITemplateRenderer, TemplateRenderer>();

            return services;
        }
    }
}
