using HotChocolate.Types;

namespace IdOps.GraphQL
{
    public class ResourceInterfaceType : InterfaceType<IResource>
    {
        protected override void Configure(IInterfaceTypeDescriptor<IResource> descriptor)
        {
            descriptor.Ignore(x => x.GetEnvironmentIds());
            descriptor.Ignore(x => x.HasEnvironments());
            descriptor.Ignore(x => x.IsInServerGroup(default!));
        }
    }
}
