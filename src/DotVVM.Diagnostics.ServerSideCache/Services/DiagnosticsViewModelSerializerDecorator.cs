using System;
using System.Collections.Generic;
using System.Linq;
using DotVVM.Framework.Controls.Infrastructure;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime.Filters;
using DotVVM.Framework.ViewModel.Serialization;
using Newtonsoft.Json.Linq;

namespace DotVVM.Diagnostics.ServerSideCache.Services
{
    public class DiagnosticsViewModelSerializerDecorator : IViewModelSerializer
    {
        private readonly DefaultViewModelSerializer defaultViewModelSerializer;
        private readonly DiagnosticsDataStore diagStore;

        public DiagnosticsViewModelSerializerDecorator(DefaultViewModelSerializer defaultViewModelSerializer, DiagnosticsDataStore diagStore)
        {
            this.defaultViewModelSerializer = defaultViewModelSerializer;
            this.diagStore = diagStore;
        }

        public void PopulateViewModel(IDotvvmRequestContext context, string serializedPostData)
        {
            var data = JObject.Parse(serializedPostData);
            if (data["viewModelCacheId"] == null)
            {
                diagStore.TrackFullPostBack(context.Route.RouteName, serializedPostData.Length);
            }

            defaultViewModelSerializer.PopulateViewModel(context, serializedPostData);
        }

        public void BuildViewModel(IDotvvmRequestContext context, object commandResult) => defaultViewModelSerializer.BuildViewModel(context, commandResult);

        public string BuildStaticCommandResponse(IDotvvmRequestContext context, object result) => defaultViewModelSerializer.BuildStaticCommandResponse(context, result);

        public string SerializeViewModel(IDotvvmRequestContext context) => defaultViewModelSerializer.SerializeViewModel(context);

        public string SerializeModelState(IDotvvmRequestContext context) => defaultViewModelSerializer.SerializeModelState(context);

        public ActionInfo ResolveCommand(IDotvvmRequestContext context, DotvvmView view) => defaultViewModelSerializer.ResolveCommand(context, view);

        public void AddPostBackUpdatedControls(IDotvvmRequestContext context, IEnumerable<(string name, string html)> postbackUpdatedControls) => defaultViewModelSerializer.AddPostBackUpdatedControls(context, postbackUpdatedControls);

        public void AddNewResources(IDotvvmRequestContext context) => defaultViewModelSerializer.AddNewResources(context);
    }
}