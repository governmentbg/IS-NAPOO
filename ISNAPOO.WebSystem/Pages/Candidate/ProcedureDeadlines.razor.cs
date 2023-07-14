using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ProcedureDeadlines : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderVM> sfGrid = new SfGrid<CandidateProviderVM>();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }
    }
}
