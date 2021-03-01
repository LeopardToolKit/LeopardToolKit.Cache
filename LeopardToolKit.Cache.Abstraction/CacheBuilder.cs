using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace LeopardToolKit.Cache
{
    public class CacheBuilder
    {
        internal CacheBuilder(IServiceCollection services)
        {
            this.Services = services;
        }
        public IServiceCollection Services { get; }
    }

    public static class CacheBuilderExtension
    {
        public static CacheBuilder AddConfiguration(this CacheBuilder cacheBuilder, IConfiguration configuration)
        {
            cacheBuilder.Services.Configure<CacheOption>(configuration);
            return cacheBuilder;
        }

        public static CacheBuilder AddConfiguration(this CacheBuilder cacheBuilder, Action<CacheOption> optionBuilder)
        {
            cacheBuilder.Services.Configure(optionBuilder);
            return cacheBuilder;
        }

        public static CacheBuilder AddCacheProvider<TCacheProvider>(this CacheBuilder cacheBuilder) 
            where TCacheProvider : ICacheProvider
        {
            cacheBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ICacheProvider), typeof(TCacheProvider)));
            return cacheBuilder;
        }

        public static CacheBuilder AddCacheProvider<TCacheProvider>(this CacheBuilder cacheBuilder, TCacheProvider cacheProvider) 
            where TCacheProvider : ICacheProvider
        {
            cacheBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ICacheProvider), cacheProvider));
            return cacheBuilder;
        }
    }
}
