using IdOps.Abstractions;

namespace IdOps.GraphQL
{
    public class RequestTokenPayload
    {
        public RequestTokenPayload(RequestTokenResult token)
        {
            Result = token;
        }

        public RequestTokenResult Result { get; }
    }
}
