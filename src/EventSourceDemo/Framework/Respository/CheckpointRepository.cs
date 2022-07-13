using EventSourceDemo.Framework.Models;
using EventStore.Client;
using StackExchange.Redis;

namespace EventSourceDemo.Framework.Respository;

public class CheckpointRepository
{

    private readonly IConnectionMultiplexer _redis;

    public CheckpointRepository(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public Task<Position?> GetAsync(string key)
    {
        var db = _redis.GetDatabase();
        
        var result = db.HashGetAll(key);
        if(!result.Any())
        {
            return Task.FromResult<Position?>(null);
        }

        var doc = result.ConvertFromRedis<CheckpointDocument>();
        return Task.FromResult<Position?>(doc.Position);
    }

    public async Task<bool> SaveAsync(string key, Position position)
    {
        var doc = new CheckpointDocument
        {
            Key = key,
            Position = position
        };
        var db = _redis.GetDatabase();
        var redisKey = new RedisKey(key);
        await db.HashSetAsync(redisKey, doc.ToHashEntries());
        return true;
    }
}