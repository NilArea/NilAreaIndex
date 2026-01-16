using Microsoft.Extensions.Logging;
using NilArea.Interfaces.IGrains;

namespace NilArea.Grains.ImplGrains;

public class AccountGrain(ILogger<AccountGrain> logger) : Grain, IAccountGrain
{
}
