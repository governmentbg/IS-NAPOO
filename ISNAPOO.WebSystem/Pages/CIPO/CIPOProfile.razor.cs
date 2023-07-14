using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Candidate.CIPO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.CIPO
{
    public partial class CIPOProfile : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderVM> applicationGrid = new SfGrid<CandidateProviderVM>();
        private CIPOApplicationModal applicationModal = new CIPOApplicationModal();

        private List<CandidateProviderVM> applicationSource = new List<CandidateProviderVM>();
        private string title = string.Empty;

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.applicationSource.Clear();
            var candidateProvider = await this.CandidateProviderService.GetActiveCandidateProviderByIdAsync(this.UserProps.IdCandidateProvider);
            if (candidateProvider is not null)
            {
                await this.applicationModal.OpenModal(candidateProvider, true, false, false, true);
            }

            this.applicationSource.Add(candidateProvider);
            this.title = candidateProvider != null ? $"Профил на ЦИПО към {candidateProvider.ProviderOwner}" : string.Empty;
        }

        private async Task UpdateAfterModalSubmit(ResultContext<CandidateProviderVM> resultContext)
        {
            if (resultContext.HasMessages)
            {
                this.applicationSource.Clear();
                this.applicationSource.Add(await this.CandidateProviderService.GetActiveCandidateProviderByIdAsync(this.UserProps.IdCandidateProvider));
                await this.applicationGrid.Refresh();

                await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
            }
            else
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
            }
        }

        private async Task EditApplication(CandidateProviderVM candidateProviderVM)
        {
            bool hasPermission = await this.CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            var candidateProvider = await this.CandidateProviderService.GetActiveCandidateProviderByIdAsync(this.UserProps.IdCandidateProvider);
            await this.applicationModal.OpenModal(candidateProvider, true, false, false, true);
        }
    }
}
