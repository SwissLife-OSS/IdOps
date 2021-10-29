using System;
using System.Collections.Generic;
using System.Linq;
using IdOps.Model;

namespace IdOps
{
    internal static class PublishingContextExtensions
    {
        public static IList<string> GetAllowedScopes(
            this IPublishingContext context,
            IEnumerable<ClientScope> scopes) =>
            context.GetAllowedScopes(scopes.Select(x => x.Id));

        public static IList<string> GetAllowedScopes(
            this IPublishingContext context,
            IEnumerable<Guid> scopeIds) =>
            scopeIds.Select(x =>
                    context.GetIdentityResourceById(x)?.Name ??
                    context.GetApiResourceById(x)?.Name ??
                    string.Empty)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
    }
}
