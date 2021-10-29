using IdOps.Security;

namespace IdOps.Core.Tests
{
    public class StubUserContextAccessor : IUserContextAccessor
    {
        public IUserContext? Context { get; set; }
    }
}
