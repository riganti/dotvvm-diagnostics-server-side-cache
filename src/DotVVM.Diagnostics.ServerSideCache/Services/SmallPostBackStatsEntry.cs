using System;
using System.Collections.Generic;
using System.Linq;

namespace DotVVM.Diagnostics.ServerSideCache.Services
{
    public class SmallPostBackStatsEntry
    {

        public string RouteName { get; set; }

        public long ClientToServerDiffBytes { get; set; }

        public long EstimatedFullPostBackBytes { get; set; }

        public int PostBackCount { get; set; }

    }
}