using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LeopardToolKit.Cache
{
    public static class MemoryCacheBuilderExtension
    {
        public static CacheBuilder AddMemoryProvider(this CacheBuilder cacheBuilder)
        {
            cacheBuilder.AddCacheProvider<MemoryCacheProvider>();
            return cacheBuilder;
        }
    }
}
