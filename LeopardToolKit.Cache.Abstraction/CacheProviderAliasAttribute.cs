using System;

namespace LeopardToolKit.Cache
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheProviderAliasAttribute : Attribute
    {
        public string Name { get; private set; }

        public CacheProviderAliasAttribute(string name)
        {
            this.Name = name;
        }
    }
}
