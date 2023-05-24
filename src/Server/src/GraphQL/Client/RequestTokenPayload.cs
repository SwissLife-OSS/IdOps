using IdOps.Abstractions;
using IdOps.Models;

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
