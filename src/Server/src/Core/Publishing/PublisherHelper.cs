using System;
using System.Collections.Generic;
using System.Linq;
using IdOps.IdentityServer.Model;

namespace IdOps
{
    public class PublisherHelper
    {
        public static TEnum MapEnum<TEnum>(Enum value)
            where TEnum : struct, Enum
        {
            var stringValue = value.ToString();

            return (TEnum)Enum.Parse(typeof(TEnum), stringValue);
        }

        public static IEnumerable<IdentityServer.Model.DataConnectorOptions> MapDataConnectors(
            ICollection<Model.DataConnectorOptions>? dataConnectors)
        {
            if (dataConnectors is null)
            {
                return Array.Empty<IdentityServer.Model.DataConnectorOptions>();
            }

            return dataConnectors.Select(x => new IdentityServer.Model.DataConnectorOptions
            {
                Enabled = x.Enabled,
                Name = x.Name,
                ProfileTypeFilter = x.ProfileTypeFilter?
                    .Select(f => MapEnum<IdentityServer.Model.ConnectorProfileType>(f)),
                Properties = x.Properties?.Select(p => new IdentityServer.Model.DataConnectorProperty
                {
                    Name = p.Name, Value = p.Value
                })
            });
        }

        public static IEnumerable<IdentityServer.Model.EnabledProvider> MapProviders(
            IEnumerable<Model.EnabledProvider>? enabledProviders)
        {
            if (enabledProviders is null)
            {
                return Array.Empty<IdentityServer.Model.EnabledProvider>();
            }

            return enabledProviders
                .Select(p => new IdentityServer.Model.EnabledProvider
                {
                    Name = p.Name,
                    RequestMfa = p.RequestMfa,
                    UserIdClaimType = p.UserIdClaimType
                });
        }

        public static PublishSource CreateSource(IResource resource)
        {
            return new PublishSource
            {
                Id = resource.Id,
                Version = resource.Version.Version,
                ModifiedAt = resource.Version.CreatedAt
            };
        }
    }
}
