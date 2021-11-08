using System;

namespace IdOps
{
    public record SaveIdentityServerRequest(
        string Name,
        Guid EnvironmentId,
        Guid GroupId,
        string Url)
    {
        public Guid? Id { get; init; }
    }
}
