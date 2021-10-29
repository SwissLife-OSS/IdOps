using System.Collections.Generic;
using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveApiScopePayload : Payload
    {
        public ApiScope? ApiScope { get; }

        public SaveApiScopePayload(ApiScope apiScope)
        {
            ApiScope = apiScope;
        }

        public SaveApiScopePayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }

    }
}
