using IdOps.Model;

namespace IdOps.GraphQL
{
    public class UpdateApplicationPayload
    {
        public UpdateApplicationPayload(Application application)
        {
            Application = application;
        }

        public Application? Application { get; }
    }
}
