using System;
using System.Collections.Generic;
using System.Linq;

namespace DotVVM.Diagnostics.ServerSideCache.Services
{
    public class ViewModelStatsEntry
    {

        public string ViewModelCacheId { get; set; }

        public string RouteName { get; set; }

        public int InMemoryStoreSizeInBytes { get; set; }

        public int JsonSizeInBytes { get; set; }

        public int ReuseCount { get; set; }
        
    }
}