using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public static class RedisCacheBuilderExtension
    {
        public static CacheBuilder AddRedisProvider(this CacheBuilder cacheBuilder, Action<RedisCacheOption> optionBuilder)
        {
            cacheBuilder.Services.Configure(optionBuilder);
            cacheBuilder.AddCacheProvider<RedisCacheProvider>();
            cacheBuilder.Services.TryAddSingleton<RedisDB>();
            return cacheBuilder;
        }

        public static CacheBuilder AddRedisProvider(this CacheBuilder cacheBuilder, IConfiguration configuration)
        {
            cacheBuilder.Services.Configure<RedisCacheOption>(configuration);
            cacheBuilder.AddCacheProvider<RedisCacheProvider>();
            cacheBuilder.Services.TryAddSingleton<RedisDB>();
            return cacheBuilder;
        }
    }
}
