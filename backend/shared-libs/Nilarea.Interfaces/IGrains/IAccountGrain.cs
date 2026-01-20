using NilArea.Contracts.Dto;
using Orleans;

namespace NilArea.Interfaces.IGrains;

[Alias("NilArea.Interfaces.IGrains.IAccountGrain")]
public interface IAccountGrain : IGrainWithGuidKey
{
    [Alias("ExistEmailAsync")]
    ValueTask<bool> ExistEmailAsync(string email);

    [Alias("RegisterUserAsync")]
    ValueTask<RegisterResponse> RegisterUserAsync(RegisterRequest request);
}
