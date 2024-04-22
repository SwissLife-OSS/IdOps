using System.Collections.Generic;

namespace IdOps.Security;

public sealed record User(string Id, string Name, IReadOnlyList<string> Roles);
