using IdOps.IdentityServer.Model;

namespace IdOps.IdentityServer;

public interface IIpWhitelistValidator
{
    bool IsValid(IdOpsClient client, out string message);
}
