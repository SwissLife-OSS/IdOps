namespace IdOps.GraphQL
{
    public class SaveIdentityServerPayload
    {
        public SaveIdentityServerPayload(Model.IdentityServer server)
        {
            Server = server;
        }

        public Model.IdentityServer Server { get; }
    }
}
