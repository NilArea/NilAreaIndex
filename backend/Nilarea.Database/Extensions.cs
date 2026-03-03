using ShardingCore.Core.ServiceProviders;
using ShardingCore.Core.ShardingConfigurations.Abstractions;

namespace Nilarea.Database;

public static class Extensions
{
    extension(IShardingRouteConfigOptions shardingRoute)
    {
        public void ConfigureRoute(IShardingProvider provider)
        {
        }
    }
}
