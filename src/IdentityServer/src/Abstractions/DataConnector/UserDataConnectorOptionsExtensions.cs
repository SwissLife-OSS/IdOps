using System;
using System.Globalization;
using System.Linq;
using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer.DataConnector
{
    public static class DataConnectorOptionsExtensions
    {
        public static TValue? GetPropertyValue<TValue>(
            this DataConnectorOptions options,
            string name)
        {
            DataConnectorProperty? property = options.Properties?
                .FirstOrDefault(x => x.Name == name);

            if (property != null)
            {
                return (TValue)Convert.ChangeType(
                    property.Value,
                    typeof(TValue),
                    CultureInfo.InvariantCulture);
            }

            return default;
        }
    }
}
