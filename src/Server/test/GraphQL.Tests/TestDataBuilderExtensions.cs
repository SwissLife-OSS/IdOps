using System;
using System.Collections.Generic;
using IdOps.Model;
using IdOps.Security;

namespace IdOps.GraphQL.Tests;

public static class TestDataBuilderExtensions
{
    public static TestDataBuilder SetupTemplate(this TestDataBuilder builder)
    {
        return builder.Enqueue(() =>
        {
            var apiScopes = new HashSet<Guid>
            {
                Guid.Parse("4986e94402f04afeb0768288457b7bf1"),
                Guid.Parse("d5684d9f86a747f187c161ed2bc3407c")
            };

            var identityScopes =
                new HashSet<Guid> { Guid.Parse("2c654c8558a44236bdd5e9ad40beb953") };

            var secret = new ClientTemplateSecret
            {
                EnvironmentId = Guid.Parse("34ca1d86dd6d4837bb9aeeeeb591fef9"),
                Type = "SharedSecret",
                Value = "asdfsadg"
            };

            var secretList = new List<ClientTemplateSecret> { secret };

            var template = new ClientTemplate
            {
                Id = Guid.Parse("443680dbca114aa09a5b6eceb1ba1671"),
                Name = "Template1",
                Tenant = "TestTenant",
                ApiScopes = apiScopes,
                IdentityScopes = identityScopes,
                Secrets = secretList
            };

            return builder.DbContext.ClientTemplates.InsertOneAsync(template);
        });
    }

    public static TestDataBuilder SetupTemplateWrongTenant(this TestDataBuilder builder)
    {
        return builder.Enqueue(() =>
        {
            var apiScopes = new HashSet<Guid>
            {
                Guid.Parse("4986e94402f04afeb0768288457b7bf1"),
                Guid.Parse("d5684d9f86a747f187c161ed2bc3407c")
            };

            var identityScopes =
                new HashSet<Guid> { Guid.Parse("2c654c8558a44236bdd5e9ad40beb953") };

            var secret = new ClientTemplateSecret
            {
                EnvironmentId = Guid.Parse("34ca1d86dd6d4837bb9aeeeeb591fef9"),
                Type = "SharedSecret",
                Value = "asdfsadg"
            };
            var secretList = new List<ClientTemplateSecret> { secret };

            var template = new ClientTemplate
            {
                Id = Guid.Parse("443680dbca114aa09a5b6eceb1ba1671"),
                Name = "Template1",
                Tenant = "Test",
                ApiScopes = apiScopes,
                IdentityScopes = identityScopes,
                Secrets = secretList
            };

            return builder.DbContext.ClientTemplates.InsertOneAsync(template);
        });
    }

    public static TestDataBuilder SetupTenant(this TestDataBuilder builder)
    {
        return builder.Enqueue(() =>
        {
            var tenant = new Tenant
            {
                Id = "TestTenant",
                Description = "testing",
                RoleMappings = new List<TenantRoleMapping>
                {
                    new() { Role = Roles.Admin, ClaimValue = Roles.Admin },
                    new() { Role = Roles.Edit, ClaimValue = Roles.Edit },
                    new() { Role = Roles.Read, ClaimValue = Roles.Read }
                }
            };

            return builder.DbContext.Tenants.InsertOneAsync(tenant);
        });
    }

    public static TestDataBuilder SetupWrongTenant(this TestDataBuilder builder)
    {
        return builder.Enqueue(() =>
        {
            var tenant = new Tenant { Id = "Test", Description = "wrong tenant" };

            return builder.DbContext.Tenants.InsertOneAsync(tenant);
        });
    }

    public static TestDataBuilder SetupApiScope(this TestDataBuilder builder)
    {
        return builder.Enqueue(() =>
        {
            var scope = new ApiScope
            {
                Id = Guid.Parse("aaa375c78dfa4730a7a22f4a805c419a"),
                Name = "scope",
                Tenant = "TestTenant",
                Enabled = true
            };

            return builder.DbContext.ApiScopes.InsertOneAsync(scope);
        });
    }
}
