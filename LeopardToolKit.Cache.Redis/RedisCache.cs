using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class RedisCache : ICache
    {
        private readonly RedisDB redisDB;

        public RedisCache(RedisDB redisDB, string categoryName)
        {
            this.redisDB = redisDB;
            this.CategoryName = categoryName;
        }

        public string CategoryName { get; }

        public T Get<T>(string key)
        {
            return this.redisDB.Get<T>(CategoryName + key);
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            this.redisDB.Set(CategoryName + key, value, absoluteExpirationTime);
        }

        public void Remove(string key)
        {
            this.redisDB.Remove(CategoryName + key);
        }
    }
}
