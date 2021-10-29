using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace IdOps.Api.Security
{
    public class JsonKeyArrayClaimAction : ClaimAction
    {
        public JsonKeyArrayClaimAction(string claimType, string jsonKey)
            : base(claimType, claimType)
        {
            JsonKey = jsonKey;
        }

        public string JsonKey { get; }

        public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
        {
            JsonElement element;

            if (userData.TryGetProperty(JsonKey, out element))
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement value in element.EnumerateArray())
                    {
                        identity.AddClaim(
                            new Claim(ClaimType, value.GetString(), ValueType, issuer));
                    }
                }
                else
                {
                    identity.AddClaim(
                        new Claim(ClaimType, element.GetString(), ValueType, issuer));
                }
            }
        }
    }
}
