using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public class ResourceRegistrationBuilder<T>
        where T : class, IResource, new()
    {
        private readonly IServiceCollection _services;

        public ResourceRegistrationBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public ResourceRegistrationBuilder<T> AddDependencyResolver<TResolver>()
            where TResolver : class, IResourceDependencyResolver
        {
            _services.AddSingleton<IResourceDependencyResolver, TResolver>();
            return this;
        }

        public ResourceRegistrationBuilder<T> AddMessageFactory<TFactory>()
            where TFactory : class, IResourceMessageFactory<T>
        {
            _services.AddSingleton<TFactory>();
            _services.AddSingleton<IResourceMessageFactory>(
                sp => sp.GetRequiredService<TFactory>());
            _services.AddSingleton<IResourceMessageFactory<T>>(
                sp => sp.GetRequiredService<TFactory>());
            return this;
        }

        public ResourceRegistrationBuilder<T> AddService<TInterface, TService>()
            where TInterface : class, IResourceService<T>
            where TService : class, TInterface
        {
            _services.AddSingleton<TService>();
            _services.AddSingleton<TInterface>(sp => sp.GetRequiredService<TService>());
            _services.AddSingleton<IResourceService>(sp => sp.GetRequiredService<TService>());
            _services.AddSingleton<IResourceService<T>>(sp => sp.GetRequiredService<TService>());
            return this;
        }
    }
}
