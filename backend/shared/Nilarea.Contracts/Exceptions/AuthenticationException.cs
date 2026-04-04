using System.Runtime.Serialization;
using NilArea.Contracts.Enums;

namespace NilArea.Contracts.Exceptions;

[Serializable]
[GenerateSerializer]
[Alias("NilArea.Contracts.Exceptions.AuthenticationException")]
public class AuthenticationException : OrleansException
{
    public AuthenticationException(AuthenticationResult result = AuthenticationResult.Default)
    {
        Result = result;
    }

    public AuthenticationException(string message, AuthenticationResult result = AuthenticationResult.Default) :
        base(message)
    {
        Result = result;
    }

    public AuthenticationException(string message, Exception innerException,
        AuthenticationResult result = AuthenticationResult.Default) : base(message, innerException)
    {
        Result = result;
    }

    [Obsolete]
    protected AuthenticationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    [Id(0)] public AuthenticationResult Result { get; set; }
}