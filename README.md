# LeopardToolKit.Cache
A useful library to let you use different cache type in same application.

##Packages

| **Name** | **Nuget** |
|----------|:-------------:|
| **LeopardToolKit.Cache.Abstraction** | **[![NuGet](https://buildstats.info/nuget/LeopardToolKit.Cache.Abstraction)](https://www.nuget.org/packages/LeopardToolKit.Cache.Abstraction)**   |
| **LeopardToolKit.Cache.Memory** | **[![NuGet](https://buildstats.info/nuget/LeopardToolKit.Cache.Memory)](https://www.nuget.org/packages/LeopardToolKit.Cache.Memory)**   |
| **LeopardToolKit.Cache.Redis** | **[![NuGet](https://buildstats.info/nuget/LeopardToolKit.Cache.Redis)](https://www.nuget.org/packages/LeopardToolKit.Cache.Redis)**   |

## Features

In your `appsettings.json` add follow:
```json
{
  "Cache": {
    "DefaultProvider": "Memory",
    "CacheCategory": [
      {
        "CacheCategory": "LeopardToolKit.Cache.Tests.CacheTest",
        "ProviderType": "Memory"
      },
      {
        "CacheCategory": "Foo",
        "ProviderType": "Memory"
      }
    ]
  }
}

```

then in `ConfigureServices`, add following:
```csharp
      services.AddCache(builder => {
                builder.AddMemoryProvider();
                builder.AddConfiguration(configuration.GetSection("Cache"));
            });
```

At last, in any place where you want to use cache, just inject following:
```csharp
ICache<SomeClass>
```
Or
```csharp
ICacheFactory cacheFactory;
...
cacheFactory.CreateCache("CacheCategory")
```