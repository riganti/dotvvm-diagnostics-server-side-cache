using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ViewModel.Serialization;
using Newtonsoft.Json.Linq;

namespace DotVVM.Diagnostics.ServerSideCache.Services
{
    public class DiagnosticsViewModelServerCache : IViewModelServerCache
    {
        private readonly DefaultViewModelServerCache baseCache;
        private readonly DiagnosticsDataStore diagStore;

        public DiagnosticsViewModelServerCache(DefaultViewModelServerCache baseCache, DiagnosticsDataStore diagStore)
        {
            this.baseCache = baseCache;
            this.diagStore = diagStore;
        }

        public string StoreViewModel(IDotvvmRequestContext context, JObject viewModelToken)
        {
            var viewModelCacheId = baseCache.StoreViewModel(context, viewModelToken);
            diagStore.TrackViewModelCreatedOrUpdated(context.Route.RouteName, viewModelCacheId, viewModelToken.ToString().Length);
            return viewModelCacheId;
        }

        public JObject TryRestoreViewModel(IDotvvmRequestContext context, string viewModelCacheId, JObject viewModelDiffToken)
        {
            try
            {
                var viewModel = baseCache.TryRestoreViewModel(context, viewModelCacheId, viewModelDiffToken);
                diagStore.TrackViewModelRestored(context.Route.RouteName, viewModelDiffToken.ToString().Length, viewModel.ToString().Length);
                return viewModel;
            }
            catch (DotvvmInterruptRequestExecutionException ex) when (ex.InterruptReason == InterruptReason.CachedViewModelMissing)
            {
                diagStore.TrackViewModelCacheMiss(context.Route.RouteName, viewModelDiffToken.ToString().Length);
                throw;
            }
        }
    }
}