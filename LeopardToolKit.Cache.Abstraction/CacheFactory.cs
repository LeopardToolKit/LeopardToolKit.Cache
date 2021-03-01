using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeopardToolKit.Cache
{
    public class CacheFactory : ICacheFactory
    {
        private readonly ConcurrentDictionary<string, ICacheProvider> providers;
        private readonly CacheOption cacheOption;

        public CacheFactory(IOptions<CacheOption> options, IEnumerable<ICacheProvider> cacheStoreProviders)
        {
            this.cacheOption = options.Value;
            this.providers = new ConcurrentDictionary<string, ICacheProvider>();
            foreach (var provider in cacheStoreProviders)
            {
                providers.TryAdd(GetProviderName(provider.GetType()), provider);
            }
        }

        public ICache CreateCache(string categoryName)
        {
            var providerType = CacheProviderSelector(categoryName);

            if(providers.TryGetValue(providerType, out var provider))
            {
                return provider.CreateCache(categoryName ?? "Default");
            }
            else
            {
                throw new Exception($"Can not find a provider for provider type: {providerType}");
            }
        }

        private string GetProviderName(Type providerType)
        {
            var attr =providerType.GetCustomAttribute<CacheProviderAliasAttribute>();
            return attr?.Name ?? providerType.FullName;
        }

        private string CacheProviderSelector(string categoryName)
        {
            if(this.cacheOption.CacheCategory == null || string.IsNullOrEmpty(categoryName))
            {
                return this.cacheOption.DefaultProvider;
            }
            var mathcedProviders = this.cacheOption.CacheCategory.Where(c => c.CacheCategory.StartsWith(categoryName));
            if (mathcedProviders.Any())
            {
                return mathcedProviders.OrderByDescending(c => c.CacheCategory.Length).First().ProviderType;
            }
            else
            {
                return this.cacheOption.DefaultProvider;
            }
        }
    }
}
