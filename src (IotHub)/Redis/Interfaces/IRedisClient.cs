using Common.Interfaces;

namespace Redis.Interfaces
{
    public interface IRedisClient : ICache
    {
        void SetObject<T>(String key, T obj, TimeSpan expiry);
        void SetString(String key, String str, TimeSpan expiry);
    }
}