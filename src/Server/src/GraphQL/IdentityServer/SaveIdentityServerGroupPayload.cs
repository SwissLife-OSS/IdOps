namespace IdOps.GraphQL
{
    public class SaveIdentityServerGroupPayload
    {
        public SaveIdentityServerGroupPayload(Model.IdentityServerGroup serverGroup)
        {
            ServerGroup = serverGroup;
        }

        public Model.IdentityServerGroup ServerGroup { get; }
    }
}
