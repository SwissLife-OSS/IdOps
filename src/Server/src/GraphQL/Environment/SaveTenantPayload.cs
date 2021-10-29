using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveEnvironmentPayload : Payload
    {
        public SaveEnvironmentPayload(Environment environment)
        {
            Environment = environment;
        }

        public Environment Environment { get; }
    }
}
