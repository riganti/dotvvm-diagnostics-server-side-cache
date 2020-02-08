using System.Collections.Concurrent;
using DotVVM.Framework.ViewModel.Serialization;

namespace DotVVM.Diagnostics.ServerSideCache.Services
{
    public class DiagnosticsViewModelServerStore : IViewModelServerStore
    {
        private readonly ConcurrentDictionary<string, byte[]> cache = new ConcurrentDictionary<string, byte[]>();

        public byte[] Retrieve(string hash)
        {
            return cache.TryGetValue(BuildKey(hash), out var data) ? data : null;
        }

        public void Store(string hash, byte[] cacheData)
        {
            cache.GetOrAdd(BuildKey(hash), cacheData);
        }

        private static string BuildKey(string hash)
        {
            return hash;
        }
    }
}