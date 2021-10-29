using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdOps.IdentityServer.Hashing
{
    /// <summary>
    /// Common extensions for <see cref="IHashAlgorithm"/>
    /// </summary>
    public static class HasherAlgorithmServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all <see cref="IHashAlgorithm"/> and the <see cref="IHashAlgorithmResolver"/>
        /// </summary>
        /// <param name="services">service collection</param>
        /// <returns></returns>
        public static IServiceCollection RegisterHashAlgorithm<T>(this IServiceCollection services)
            where T : class, IHashAlgorithm
        {
            services.AddSingleton<IHashAlgorithm, T>();
            services.TryAddSingleton<IHashAlgorithmResolver, HashAlgorithmResolver>();
            return services;
        }

        /// <summary>
        /// Registers all <see cref="IHashAlgorithm"/> and the <see cref="IHashAlgorithmResolver"/>
        /// </summary>
        /// <param name="services">service collection</param>
        /// <returns></returns>
        public static IServiceCollection RegisterHashAlgorithms(this IServiceCollection services)
        {
            services.AddSingleton<IPasswordHasher<string>, PasswordHasher<string>>();
            services.RegisterHashAlgorithm<Pbkdf2HashAlgorithm>();
            services.RegisterHashAlgorithm<SshaHashAlgorithm>();
            return services;
        }
    }
}
