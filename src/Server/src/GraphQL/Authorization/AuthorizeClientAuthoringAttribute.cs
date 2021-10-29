using System.Reflection;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using IdOps.Authorization;

namespace IdOps.GraphQL
{
    public class AuthorizeClientAuthoringAttribute : ObjectFieldDescriptorAttribute
    {
        private readonly AccessMode _mode;
        private readonly bool _includeTenantAuth;

        public AuthorizeClientAuthoringAttribute(AccessMode mode, bool includeTenantAuth)
        {
            _mode = mode;
            _includeTenantAuth = includeTenantAuth;
        }

        public AuthorizeClientAuthoringAttribute(AccessMode mode)
            : this(mode, true)
        {
        }

        public override void OnConfigure(
            IDescriptorContext context,
            IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            if (_includeTenantAuth)
            {
                descriptor
                    .Authorize(AuthorizationPolicies.Names.TenantResourceAccess, ApplyPolicy.AfterResolver);
            }

            if (_mode == AccessMode.Write)
            {
                descriptor
                    .Authorize(AuthorizationPolicies.Names.ResourceAuthoringEdit, ApplyPolicy.BeforeResolver);
            }
            else
            {
                descriptor
                    .Authorize(AuthorizationPolicies.Names.ResourceAuthoringRead, ApplyPolicy.BeforeResolver);
            }
        }
    }
    public class IdOpsAuthorization : DescriptorAttribute
    {
        protected override void TryConfigure(
            IDescriptorContext context,
            IDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            if (descriptor is IObjectTypeDescriptor objectTypeDescriptor)
            {
                TryConfigure(context, objectTypeDescriptor, element);
            }
            else if (descriptor is IObjectFieldDescriptor objectFieldDescriptor)
            {
                TryConfigure(context, objectFieldDescriptor, element);
            }
        }

        private void TryConfigure(
            IDescriptorContext context,
            IObjectTypeDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            descriptor
                .Authorize(AuthorizationPolicies.Names.ResourceAuthoringEdit, ApplyPolicy.BeforeResolver);
        }

        private void TryConfigure(
            IDescriptorContext context,
            IObjectFieldDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            descriptor
                .Authorize(AuthorizationPolicies.Names.ResourceAuthoringEdit, ApplyPolicy.BeforeResolver);
        }
    }

}
