using System.Collections.Generic;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveApiResourcePayload : Payload
    {
        public ApiResource? ApiResource { get; }

        public SaveApiResourcePayload(ApiResource apiResource)
        {
            ApiResource = apiResource;
        }

        public SaveApiResourcePayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }

    public class AddApiSecretPayload
    {
        public AddApiSecretPayload(ApiResource apiResource, string secret)
        {
            ApiResource = apiResource;
            Secret = secret;
        }

        public ApiResource ApiResource { get; }
        public string Secret { get; }
    }
}
