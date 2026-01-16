using Orleans;

namespace NilArea.Interfaces.IGrains;

[Alias("NilArea.Interfaces.IGrains.IAccountGrain")]
public interface IAccountGrain : IGrainWithGuidKey
{
}
