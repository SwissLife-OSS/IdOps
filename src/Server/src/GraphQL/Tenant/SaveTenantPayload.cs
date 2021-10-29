using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveTenantPayload : Payload
    {
        public SaveTenantPayload(Tenant tenant)
        {
            Tenant = tenant;
        }

        public Tenant Tenant { get; }
    }
}
