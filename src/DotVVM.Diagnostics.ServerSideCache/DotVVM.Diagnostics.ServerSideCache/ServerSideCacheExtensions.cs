using System;
using System.Linq;
using DotVVM.Diagnostics.ServerSideCache.Services;
using DotVVM.Framework.Configuration;
using DotVVM.Framework.ViewModel.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotVVM.Diagnostics.ServerSideCache
{
    public static class ServerSideCacheExtensions
    {

        /// <summary>
        /// Adds Server Side ViewModel Cache Diagnostics to the application.
        /// </summary>
        public static IDotvvmServiceCollection AddServerSideCacheDiagnostics(this IDotvvmServiceCollection services, ServerSideCacheDiagnosticsOptions options = null)
        {
            if (options == null)
            {
                options = ServerSideCacheDiagnosticsOptions.CreateDefaultOptions();
            }
            services.Services.AddSingleton<ServerSideCacheDiagnosticsOptions>(options);
            services.Services.AddSingleton<DiagnosticsDataStore>();

            services.Services.AddSingleton<IViewModelServerStore, DiagnosticsViewModelServerStore>();
            services.Services.AddSingleton<IViewModelServerCache, DiagnosticsViewModelServerCache>();

            services.Services.AddSingleton<DefaultViewModelSerializer>();
            services.Services.AddSingleton<DefaultViewModelServerCache>();
            services.Services.Replace(ServiceDescriptor.Singleton<IViewModelSerializer>(provider => new DiagnosticsViewModelSerializerDecorator(
                provider.GetService<DefaultViewModelSerializer>(),
                provider.GetService<DiagnosticsDataStore>()
            )));

            services.Services.Configure((DotvvmConfiguration config) =>
            {
                config.RouteTable.Add(options.RouteName, options.Url, "embedded://DotVVM.Diagnostics.ServerSideCache/Pages.Report.dothtml");
            });

            return services;
        }
    }
}
