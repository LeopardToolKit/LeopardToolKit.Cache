using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeopardToolKit.Cache
{
    public class RedisDB : IDisposable
    {
        private readonly static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

        private readonly ConnectionMultiplexer _redis;
        private readonly RedisCacheOption _cacheOption;

        public RedisDB(IOptions<RedisCacheOption> options)
        {
            var config = ConfigurationOptions.Parse(options.Value.RedisConnection);
            config.ConnectRetry = 3;
            config.SyncTimeout = 10;
            config.ConnectTimeout = 15000;
            _redis = ConnectionMultiplexer.Connect(config);
            _cacheOption = options.Value;
        }

        internal T Get<T>(string key)
        {
            IDatabase db = _redis.GetDatabase(_cacheOption.DataBaseIndex);
            string json = db.StringGet(key);
            if(typeof(T) == typeof(string))
            {
                return (T)(object)json;
            }

            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            else
            {
                return JsonSerializer.Deserialize<T>(json, _serializerOptions);
            }
        }

        internal void Set<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            var json = JsonSerializer.Serialize(value, _serializerOptions);
            IDatabase db = _redis.GetDatabase(_cacheOption.DataBaseIndex);
            db.StringSet(key, json, absoluteExpirationTime);
        }

        internal void Remove(string key)
        {
            IDatabase db = _redis.GetDatabase(_cacheOption.DataBaseIndex);
            db.KeyDelete(key);
        }

        public void Dispose()
        {
            _redis.Dispose();
        }
    }
}
