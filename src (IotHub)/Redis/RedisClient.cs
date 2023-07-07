using Common.Contracts.Config;
using Common.Interfaces;
using Newtonsoft.Json;
using Redis.Interfaces;
using StackExchange.Redis;

namespace Redis
{
    /// <summary>
    /// https://metanit.com/sharp/aspnet6/17.2.php
    /// </summary>
    public class RedisClient : IRedisClient
    {
        private readonly CacheConfig _config;
        private readonly ConnectionMultiplexer _connection;


        public RedisClient(ConnectionMultiplexer connection, CacheConfig config)
        {
            _connection = connection;
            _config = config;
        }


        // ICache /////////////////////////////////////////////////////////////////////////////////
        public Boolean IsCachingEnabled => _config.IsCachingEnabled;

        void ICache.SetString(String key, String str)
        {
            if (_config.IsCachingEnabled)
            {
                _connection.GetDatabase().StringSet(key, str);
            }
        }
        public void SetString(String key, String str)
        {
            SetString(key, str, TimeSpan.FromSeconds(_config.DefaultExpirySec));
        }
        public void Set<T>(String key, T obj)
        {
            SetString(key, JsonConvert.SerializeObject(obj));
        }

        public String GetString(String key)
        {
            return _connection.GetDatabase().StringGet(key);
        }
        public T Get<T>(String key)
        {
            return JsonConvert.DeserializeObject<T>(GetString(key));
        }
        public Boolean TryGetString(String key, out String str)
        {
            str = GetString(key);
            return !string.IsNullOrEmpty(str);
        }
        public Boolean TryGet<T>(String key, out T obj)
        {
            if (TryGetString(key, out var str))
            {
                obj = JsonConvert.DeserializeObject<T>(GetString(key));
                return true;
            }

            obj = default;
            return false;
        }

        public void Remove(String key)
        {
            _connection.GetDatabase().KeyDelete(key);
        }


        // IRedisClient ///////////////////////////////////////////////////////////////////////////
        public void SetString(String key, String str, TimeSpan expiry)
        {
            _connection.GetDatabase().StringSet(key, str, expiry);
        }
        public void SetObject<T>(String key, T obj, TimeSpan expiry)
        {
            SetString(key, JsonConvert.SerializeObject(obj), expiry);
        }
    }
}