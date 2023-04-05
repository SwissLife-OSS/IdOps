﻿using IdOps.Abstractions;
using IdOps.Models;
using Microsoft.Extensions.DependencyInjection;

namespace IdOps;

public static class AuthTokenGeneratorExtension
{
    public static IServiceCollection AddAuthTokenGenerator(this IServiceCollection services)
    {
        services.AddSingleton<IDefaultShellService, DefaultShellService>();
            services.AddSingleton<ITokenAnalyzer, TokenAnalyzer>();
            services.AddSingleton<ISettingsStore, SettingsStore>();
            services.AddSingleton<IUserSettingsManager, UserSettingsManager>();
            //services.AddSingleton<IAuthorizeRequestService, AuthorizeRequestService>();

            //services.AddSingleton<IIdentityRequestStore, LocalIdentityRequestStore>();
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<IAuthTokenStore, UserDataAuthTokenStore>();


        return services;
    }
}