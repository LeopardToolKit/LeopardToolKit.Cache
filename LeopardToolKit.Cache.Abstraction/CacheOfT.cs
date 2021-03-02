using System;


namespace LeopardToolKit.Cache
{
    public class Cache<TCategory> : ICache<TCategory> where TCategory : class
    {
        private ICache _innerCache;

        public Cache(ICacheFactory cacheFactory)
        {
            this.CategoryName = typeof(TCategory).FullName;
            this._innerCache = cacheFactory.CreateCache(this.CategoryName);
            
        }

        public string CategoryName { get; }

        public T Get<T>(string key)
        {
            return this._innerCache.Get<T>(key);
        }

        public void Put<T>(string key, T value, TimeSpan absoluteExpirationTime)
        {
            this._innerCache.Put(key, value, absoluteExpirationTime);
        }

        public void Remove(string key)
        {
            this._innerCache.Remove(key);
        }
    }
}
