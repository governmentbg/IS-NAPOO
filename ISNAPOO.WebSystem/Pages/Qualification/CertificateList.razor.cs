using System;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Qualification
{
    public partial class CertificateList : BlazorBaseComponent
    {
        //change CandidateProviderVM
        private SfGrid<CandidateProviderVM> sfGrid = new SfGrid<CandidateProviderVM>();
        private IEnumerable<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();

        CertificateModal modal;

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            candidateProviders = await CandidateProviderService.GetAllCandidateProvidersAsync();
        }

        public async Task SelectedRow(CandidateProviderVM candidate)
        {
            if (this.loading) return;

            try
            {
                this.loading = true;
                modal.openModal(candidate);

            }
            finally {
                this.loading = false;
            }

        }
    }
}

