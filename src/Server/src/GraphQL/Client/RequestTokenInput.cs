using System;

namespace IdOps.GraphQL;

public class RequestTokenInput
{
  public string Authority { get; set; }
  public string ClientId { get; set; }
  public string SecretId { get; set; }
  public string GrantType { get; set; }
}
