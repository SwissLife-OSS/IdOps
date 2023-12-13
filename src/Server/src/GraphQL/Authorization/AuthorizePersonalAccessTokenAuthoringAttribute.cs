using System.Reflection;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Authorization;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using IdOps.Authorization;

namespace IdOps.GraphQL
{
    public class AuthorizePersonalAccessTokenAuthoringAttribute : ObjectFieldDescriptorAttribute
    {
        private readonly AccessMode _mode;
        private readonly bool _includeTenantAuth;

        public AuthorizePersonalAccessTokenAuthoringAttribute(
            AccessMode mode,
            bool includeTenantAuth)
        {
            _mode = mode;
            _includeTenantAuth = includeTenantAuth;
        }

        public AuthorizePersonalAccessTokenAuthoringAttribute(AccessMode mode)
            : this(mode, false)
        {
        }

        protected override void OnConfigure(
            IDescriptorContext context,
            IObjectFieldDescriptor descriptor,
            MemberInfo member)
        {
            if (_includeTenantAuth)
            {
                descriptor
                    .Authorize(AuthorizationPolicies.Names.TenantResourceAccess,
                        ApplyPolicy.AfterResolver);
            }

            if (_mode == AccessMode.Write)
            {
                descriptor
                    .Authorize(AuthorizationPolicies.Names.PersonalAccessTokenEdit);
            }
            else
            {
                descriptor
                    .Authorize(AuthorizationPolicies.Names.PersonalAccessTokenRead);
            }
        }
    }
}
