using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotVVM.Diagnostics.ServerSideCache.DTO;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.ViewModel.Serialization;

namespace DotVVM.Diagnostics.ServerSideCache.Services
{
    public class RouteReportEntry
    {
        public string RouteName { get; set; }
        public int TotalViewModelInstances { get; set; }
        public int TotalInMemorySize { get; set; }
        public double AverageViewModelReuse { get; set; }
    }

    public class DiagnosticsDataStore
    {
        private readonly IViewModelServerStore store;
        private readonly ServerSideCacheDiagnosticsOptions options;
        private readonly DotvvmConfiguration dotvvmConfiguration;

        private readonly object viewModelEntriesLocker = new object();
        private readonly Dictionary<ViewModelCacheKey, ViewModelStatsEntry> viewModelEntries = new Dictionary<ViewModelCacheKey, ViewModelStatsEntry>();

        private readonly object smallPostBackEntriesLocker = new object();
        private readonly Dictionary<string, SmallPostBackStatsEntry> smallPostBackEntries = new Dictionary<string, SmallPostBackStatsEntry>();

        private readonly object fullPostBackEntriesLocker = new object();
        private readonly Dictionary<string, FullPostBackStatsEntry> fullPostBackEntries = new Dictionary<string, FullPostBackStatsEntry>();


        public DiagnosticsDataStore(IViewModelServerStore store, ServerSideCacheDiagnosticsOptions options, DotvvmConfiguration dotvvmConfiguration)
        {
            this.store = store;
            this.options = options;
            this.dotvvmConfiguration = dotvvmConfiguration;
        }


        public void TrackViewModelCreatedOrUpdated(string routeName, string viewModelCacheId, int jsonLength)
        {
            if (routeName == options.RouteName)
            {
                return;
            }

            lock (viewModelEntriesLocker)
            {
                var cacheKey = new ViewModelCacheKey(routeName, viewModelCacheId);
                if (!viewModelEntries.TryGetValue(cacheKey, out var entry))
                {
                    entry = new ViewModelStatsEntry()
                    {
                        RouteName = routeName,
                        ViewModelCacheId = viewModelCacheId,
                        InMemoryStoreSizeInBytes = store.Retrieve(viewModelCacheId).Length,
                        JsonSizeInBytes = jsonLength,
                        ReuseCount = 1
                    };
                    viewModelEntries.Add(cacheKey, entry);
                }
                else
                {
                    entry.ReuseCount++;
                }
            }
        }

        public void TrackViewModelRestored(string routeName, int diffJsonLength, int estimatedFullJsonLength)
        {
            if (routeName == options.RouteName)
            {
                return;
            }

            lock (smallPostBackEntriesLocker)
            {
                if (!smallPostBackEntries.TryGetValue(routeName, out var entry))
                {
                    entry = new SmallPostBackStatsEntry()
                    {
                        RouteName = routeName,
                        ClientToServerDiffBytes = diffJsonLength,
                        EstimatedFullPostBackBytes = estimatedFullJsonLength,
                        PostBackCount = 1
                    };
                    smallPostBackEntries.Add(routeName, entry);
                }
                else
                {
                    entry.ClientToServerDiffBytes += diffJsonLength;
                    entry.EstimatedFullPostBackBytes += estimatedFullJsonLength;
                    entry.PostBackCount++;
                }
            }
        }

        public void TrackFullPostBack(string routeName, int jsonLength)
        {
            if (routeName == options.RouteName)
            {
                return;
            }

            lock (fullPostBackEntriesLocker)
            {
                if (!fullPostBackEntries.TryGetValue(routeName, out var entry))
                {
                    entry = new FullPostBackStatsEntry()
                    {
                        RouteName = routeName,
                        FullPostBackBytes = jsonLength,
                        FullPostBackCount = 1
                    };
                    fullPostBackEntries.Add(routeName, entry);
                }
                else
                {
                    entry.FullPostBackBytes += jsonLength;
                    entry.FullPostBackCount++;
                }
            }
        }

        public void TrackViewModelCacheMiss(string routeName, int diffJsonLength)
        {
            if (routeName == options.RouteName)
            {
                return;
            }

            lock (fullPostBackEntriesLocker)
            {
                if (!fullPostBackEntries.TryGetValue(routeName, out var entry))
                {
                    entry = new FullPostBackStatsEntry()
                    {
                        RouteName = routeName,
                        CacheMissDiffBytes = diffJsonLength,
                        CacheMissPostBackCount = 1
                    };
                    fullPostBackEntries.Add(routeName, entry);
                }
                else
                {
                    entry.CacheMissDiffBytes += diffJsonLength;
                    entry.CacheMissPostBackCount++;
                }
            }
        }

        public IEnumerable<RouteStatsDTO> GetReport()
        {
            var routes = GetRouteReportEntries();

            lock (smallPostBackEntriesLocker)
            lock (fullPostBackEntriesLocker)
            {
                return routes
                    .Select(r =>
                    {
                        smallPostBackEntries.TryGetValue(r.RouteName, out var smallPostBackStatsEntry);
                        fullPostBackEntries.TryGetValue(r.RouteName, out var fullPostBackStatsEntry);

                        return new RouteStatsDTO()
                        {
                            RouteName = r.RouteName,
                            TotalViewModelInstances = r.TotalViewModelInstances,
                            TotalInMemorySize = r.TotalInMemorySize,
                            AverageViewModelReuse = r.AverageViewModelReuse,
                            TotalClientToServerDiffSize = smallPostBackStatsEntry?.ClientToServerDiffBytes ?? 0,
                            TotalEstimatedFullPostBackSize = smallPostBackStatsEntry?.EstimatedFullPostBackBytes ?? 0,
                            SmallPostBackCount = smallPostBackStatsEntry?.PostBackCount ?? 0,
                            TotalCacheMissDiffBytes = fullPostBackStatsEntry?.CacheMissDiffBytes ?? 0,
                            TotalFullPostBackBytes = fullPostBackStatsEntry?.FullPostBackBytes ?? 0,
                            CacheMissPostBackCount = fullPostBackStatsEntry?.CacheMissPostBackCount ?? 0,
                            FullPostBackCount = fullPostBackStatsEntry?.FullPostBackCount ?? 0
                        };
                    })
                    .ToList();
            }
        }

        private List<RouteReportEntry> GetRouteReportEntries()
        {
            List<RouteReportEntry> entries;
            lock (viewModelEntriesLocker)
            {
                entries = viewModelEntries
                    .GroupBy(r => r.Key.RouteName)
                    .Select(r => new RouteReportEntry
                    {
                        RouteName = r.Key, 
                        TotalViewModelInstances = r.Count(), 
                        TotalInMemorySize = r.Sum(vm => vm.Value.InMemoryStoreSizeInBytes),
                        AverageViewModelReuse = r.Average(vm => (double) vm.Value.ReuseCount)
                    })
                    .ToList();
            }

            foreach (var route in dotvvmConfiguration.RouteTable.Where(r => r.RouteName != options.RouteName && !entries.Any(i => i.RouteName == r.RouteName)))
            {
                entries.Add(new RouteReportEntry() { RouteName = route.RouteName });
            }

            return entries;
        }

        struct ViewModelCacheKey
        {
            public string RouteName { get; }
            public string ViewModelId { get; }

            public ViewModelCacheKey(string routeName, string viewModelId)
            {
                RouteName = routeName;
                ViewModelId = viewModelId;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public bool Equals(ViewModelCacheKey other)
            {
                return RouteName == other.RouteName && ViewModelId == other.ViewModelId;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((RouteName != null ? RouteName.GetHashCode() : 0) * 397) ^ (ViewModelId != null ? ViewModelId.GetHashCode() : 0);
                }
            }
        }
    }
}
