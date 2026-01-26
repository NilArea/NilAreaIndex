using StackExchange.Redis;

namespace NilArea.Grains.DbContext;

public interface IRedisDatabaseFactory
{
    IDatabase GetDatabase(int? db = null);
    IServer GetServer(string endpoint);
}

public class RedisDatabaseFactory(IConnectionMultiplexer connectionMultiplexer) : IRedisDatabaseFactory
{
    public IDatabase GetDatabase(int? db = null)
    {
        return connectionMultiplexer.GetDatabase(db ?? -1);
    }

    public IServer GetServer(string endpoint)
    {
        return connectionMultiplexer.GetServer(endpoint);
    }
}
