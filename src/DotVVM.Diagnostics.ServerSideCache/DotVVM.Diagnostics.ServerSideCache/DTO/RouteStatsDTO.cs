using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace DotVVM.Diagnostics.ServerSideCache.DTO
{
    public class RouteStatsDTO
    {

        public string RouteName { get; set; }

        public int TotalViewModelInstances { get; set; }

        public long TotalInMemorySize { get; set; }

        public double AverageViewModelReuse { get; set; }

        public long TotalClientToServerDiffSize { get; set; }

        public long TotalEstimatedFullPostBackSize { get; set; }

        public long TotalEstimatedSavedSize => TotalEstimatedFullPostBackSize - TotalClientToServerDiffSize;

        public long TotalCacheMissDiffBytes { get; set; }

        public long TotalFullPostBackBytes { get; set; }

        public int SmallPostBackCount { get; set; }

        public int CacheMissPostBackCount { get; set; }

        public int FullPostBackCount { get; set; }

        public double DataTransferEfficiencyScore
        {
            get
            {
                var totalViewModelSize = TotalEstimatedFullPostBackSize + TotalFullPostBackBytes;
                if (totalViewModelSize == 0)
                {
                    return 0;
                }

                // what has actually been transferred vs what would have been transferred without cache
                return (1 - (TotalClientToServerDiffSize + TotalCacheMissDiffBytes + TotalFullPostBackBytes) / (double)totalViewModelSize) * 100;
            }
        }
    }
}
