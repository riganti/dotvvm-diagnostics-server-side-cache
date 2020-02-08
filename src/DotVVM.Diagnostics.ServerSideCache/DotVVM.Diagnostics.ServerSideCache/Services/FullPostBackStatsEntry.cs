using System;
using System.Collections.Generic;
using System.Linq;

namespace DotVVM.Diagnostics.ServerSideCache.Services
{
    public class FullPostBackStatsEntry
    {

        public string RouteName { get; set; }

        public long CacheMissDiffBytes { get; set; }

        public long FullPostBackBytes { get; set; }

        public int CacheMissPostBackCount { get; set; }

        public int FullPostBackCount { get; set; }

    }
}