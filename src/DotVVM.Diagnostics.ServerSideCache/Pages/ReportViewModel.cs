using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Diagnostics.ServerSideCache.DTO;
using DotVVM.Diagnostics.ServerSideCache.Services;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;

namespace DotVVM.Diagnostics.ServerSideCache.Pages
{
    public class ReportViewModel : DotvvmViewModelBase
    {
        private readonly DiagnosticsDataStore diagnosticsDataStore;

        public GridViewDataSet<RouteStatsDTO> RouteStats { get; set; } = new GridViewDataSet<RouteStatsDTO>()
        {
            PagingOptions =
            {
                PageSize = 50
            },
            SortingOptions =
            {
                SortExpression = nameof(RouteStatsDTO.DataTransferEfficiencyScore),
                SortDescending = false
            }
        };

        public ReportViewModel(DiagnosticsDataStore diagnosticsDataStore)
        {
            this.diagnosticsDataStore = diagnosticsDataStore;
        }

        public override Task PreRender()
        {
            if (RouteStats.IsRefreshRequired)
            {
                RouteStats.LoadFromQueryable(diagnosticsDataStore.GetReport().AsQueryable());
            }

            return base.PreRender();
        }
    }
}

