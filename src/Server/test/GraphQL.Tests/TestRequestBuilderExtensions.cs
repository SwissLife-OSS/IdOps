using IdentityModel;

namespace IdOps.GraphQL.Tests;

public static class TestRequestBuilderExtensions
{
    public static ITestRequestBuilder AddUser(this ITestRequestBuilder builder)
    {
        builder.AddClaim(JwtClaimTypes.Subject, "test-user");
        builder.AddClaim(JwtClaimTypes.Name, "Test User");
        builder.SetAuthenticated(true);

        return builder;
    }

    public static ITestRequestBuilder AddRole(
        this ITestRequestBuilder builder,
        string? role)
    {
        if (role is not null)
        {
            builder.AddClaim(JwtClaimTypes.Role, role);
        }

        return builder;
    }
}
