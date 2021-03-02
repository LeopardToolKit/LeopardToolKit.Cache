using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace LeopardToolKit.Cache.Tests
{
    [TestClass]
    public class CacheTest
    {
        [TestMethod]
        public async Task TestCacheDefault()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddMemoryProvider();
                builder.AddConfiguration(b => b.DefaultProvider = "Memory");
            });
            var provider = services.BuildServiceProvider();
            var cacheFactory = provider.GetRequiredService<ICacheFactory>();
            var cache = provider.GetRequiredService<ICache<CacheTest>>();
            int value = 2;
            string key = "int";
            cache.Put(key, value, TimeSpan.FromSeconds(3));
            var cachedValue = cache.Get<int>(key);
            Assert.AreEqual(value, cachedValue);
            await Task.Delay(5000);
            cachedValue = cache.Get<int>(key);
            Assert.AreEqual(0, cachedValue);
        }

        [TestMethod]
        public void TestCacheDifferentForDifferentCategory()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddMemoryProvider();
                builder.AddConfiguration(b => b.DefaultProvider = "Memory");
            });
            var provider = services.BuildServiceProvider();
            var cacheFactory = provider.GetRequiredService<ICacheFactory>();
            var cache1 = cacheFactory.CreateCache("Test");
            var cache2 = provider.GetRequiredService<ICache<CacheTest>>();
            Assert.IsFalse(cache1 == cache2);
        }

        [TestMethod]
        public void TestCache()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddMemoryProvider();
                builder.AddConfiguration(b => b.DefaultProvider = "Memory");
            });
            var provider = services.BuildServiceProvider();
            var cacheFactory = provider.GetRequiredService<ICacheFactory>();
            var cache1 = cacheFactory.CreateCache("Test");
            var cache2 = provider.GetRequiredService<ICache<CacheTest>>();
            Assert.IsFalse(cache1 == cache2);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestSetting()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("testsettings.json");
            var configuration = configurationBuilder.Build();
            ServiceCollection services = new ServiceCollection();
            services.AddCache(builder => {
                builder.AddMemoryProvider();
                builder.AddConfiguration(configuration.GetSection("Cache"));
            });
            var provider = services.BuildServiceProvider();
            var cacheFactory = provider.GetRequiredService<ICacheFactory>();
            var cache1 = cacheFactory.CreateCache("Foo");
            var cache2 = provider.GetRequiredService<ICache<CacheTest>>();
            Assert.IsFalse(cache1 == cache2);

            cacheFactory.CreateCache("Bar");
        }
    }
}
