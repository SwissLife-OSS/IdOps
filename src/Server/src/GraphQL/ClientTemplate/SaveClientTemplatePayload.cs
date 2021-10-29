using IdOps.Model;

namespace IdOps.GraphQL
{
    public class SaveClientTemplatePayload
    {
        public SaveClientTemplatePayload(ClientTemplate clientTemplate)
        {
            ClientTemplate = clientTemplate;
        }

        public ClientTemplate? ClientTemplate { get; }
    }
}
