using System.Runtime.Serialization;

namespace NilArea.Interfaces.Exceptions;

[Serializable]
[GenerateSerializer]
[Alias("NilArea.Interfaces.Exceptions.AuthenticationException")]
public class AuthenticationException : OrleansException
{
    public AuthenticationException()
    {
    }

    public AuthenticationException(string message) : base(message)
    {
    }

    public AuthenticationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    [Obsolete]
    protected AuthenticationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
