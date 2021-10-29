using Microsoft.Extensions.DependencyInjection;

namespace IdOps
{
    public static class ResourceRegistrationBuilderServiceCollectionExtensions
    {
        public static ResourceRegistrationBuilder<T> AddResource<T>(
            this IServiceCollection services)
            where T : class, IResource, new()
        {
            return new ResourceRegistrationBuilder<T>(services);
        }

        public static void Foo(this IServiceCollection services)
        {
        }
    }
}
