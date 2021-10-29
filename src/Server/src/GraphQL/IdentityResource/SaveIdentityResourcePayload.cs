using System.Collections.Generic;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveIdentityResourcePayload : Payload
    {
        public IdentityResource? IdentityResource { get; }

        public SaveIdentityResourcePayload(IdentityResource identityResource)
        {
            IdentityResource = identityResource;
        }

        public SaveIdentityResourcePayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
