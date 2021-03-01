using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    [CacheProviderAlias("Redis")]
    public class RedisCacheProvider : ICacheProvider
    {
        private readonly RedisDB redisDB;

        public RedisCacheProvider(RedisDB redisDB)
        {
            this.redisDB = redisDB;
        }

        public ICache CreateCache(string name)
        {
            return new RedisCache(this.redisDB, name);
        }
    }
}
