using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.Hosting;

namespace DotVVM.Diagnostics.ServerSideCache
{
    public class ServerSideCacheDiagnosticsOptions
    {

        public string RouteName { get; set; } = "_ServerSideCacheReport";

        public string Url { get; set; } = "_diagnostics/cache";

        public Func<IDotvvmRequestContext, Task<bool>> OnAuthorize { get; set; }
            = context => Task.FromResult(context.HttpContext.Request.Url.IsLoopback);

        public static ServerSideCacheDiagnosticsOptions CreateDefaultOptions()
        {
            return new ServerSideCacheDiagnosticsOptions();
        }

    }
}