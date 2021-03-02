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
        private readonly ConcurrentDictionary<string, ICacheProvider> _categoryProvidersMapping;
        private readonly CacheOption _cacheOption;
        private readonly IEnumerable<ICacheProvider> _cacheProviders;

        public CacheFactory(IOptions<CacheOption> options, IEnumerable<ICacheProvider> cacheProviders)
        {
            this._cacheOption = options.Value;
            this._categoryProvidersMapping = new ConcurrentDictionary<string, ICacheProvider>();
            this._cacheProviders = cacheProviders;
        }

        public ICache CreateCache(string categoryName)
        {
            if(_categoryProvidersMapping.TryGetValue(categoryName, out var provider))
            {
                return provider.CreateCache(categoryName ?? "Default");
            }
            else
            {
                provider = CacheProviderSelector(categoryName);
                if(provider != null)
                {
                    _categoryProvidersMapping.TryAdd(categoryName, provider);
                    return provider.CreateCache(categoryName ?? "Default");
                }
                else
                {
                    throw new ArgumentException($"Cannot find any cache provider for the category {categoryName}");
                }
            }
        }

        private ICacheProvider CacheProviderSelector(string categoryName)
        {
            string providerName;
            if(this._cacheOption.CacheCategories == null || string.IsNullOrEmpty(categoryName))
            {
                providerName = this._cacheOption.DefaultProvider;
            }
            else
            {
                var mathcedCategories = this._cacheOption.CacheCategories.Where(c => c.Key.StartsWith(categoryName));
                if (mathcedCategories.Any())
                {
                    providerName = mathcedCategories.OrderByDescending(c => c.Key.Length).First().Value;
                }
                else
                {
                    providerName = this._cacheOption.DefaultProvider;
                }
            }

            return _cacheProviders.FirstOrDefault(p => {
                var providerType = p.GetType();
                if(providerType.Name == providerName)
                {
                    return true;
                }

                var aliasAttr = providerType.GetCustomAttribute<CacheProviderAliasAttribute>();
                if(aliasAttr == null)
                {
                    return false;
                }
                else
                {
                    return aliasAttr.Name == providerName;
                }
            });
        }
    }
}
