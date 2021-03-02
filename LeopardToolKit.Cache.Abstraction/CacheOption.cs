using System;
using System.Collections.Generic;
using System.Text;

namespace LeopardToolKit.Cache
{
    public class CacheOption
    {
        public string DefaultProvider { get; set; }

        public Dictionary<string,string> CacheCategory { get; set; }
    }
}
