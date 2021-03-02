using System.Collections.Generic;

namespace LeopardToolKit.Cache
{
    public class CacheOption
    {
        public string DefaultProvider { get; set; }

        public Dictionary<string,string> CacheCategory { get; set; }
    }
}
