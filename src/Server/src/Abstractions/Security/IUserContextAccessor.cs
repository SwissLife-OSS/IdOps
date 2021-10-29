using System;
using System.Diagnostics.CodeAnalysis;

namespace IdOps.Security
{
    public interface IUserContextAccessor
    {
        IUserContext? Context { get; set; }
    }

    public class CouldNotAccessUserContextException : Exception
    {
        public CouldNotAccessUserContextException() : base("Could not access user context")
        {

        }

        public static Exception New() => throw new CouldNotAccessUserContextException();
    }
}
