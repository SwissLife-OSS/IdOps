using System;
using System.Diagnostics.CodeAnalysis;

namespace IdOps
{
    public class ErrorException : Exception
    {
        public ErrorException(IError error)
        {
            Error = error;
        }

        public IError Error { get; }

        [DoesNotReturn]
        public static void Throw(IError error)
        {
            throw new ErrorException(error);
        }
    }
}
