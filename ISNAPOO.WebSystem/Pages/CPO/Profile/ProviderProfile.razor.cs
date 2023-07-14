using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.CPO.Profile
{
    public partial class ProviderProfile : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderVM> applicationGrid = new SfGrid<CandidateProviderVM>();
        private ApplicationModal applicationModal = new ApplicationModal();

        private List<CandidateProviderVM> applicationSource = new List<CandidateProviderVM>();
        private string title = string.Empty;
        private bool isCPO = true;

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                await this.HandleTokenData();

                this.SpinnerHide();
            }
        }

        private async Task HandleTokenData(bool invokedAfterApplicatioModalSubmit = false)
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    var entryFrom = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();
                    if (entryFrom == GlobalConstants.TOKEN_CPO_PROFILE_VALUE)
                    {
                        this.isCPO = true;

                        var candidateProvider = await this.CandidateProviderService.GetActiveCandidateProviderWithLocationIncludedByIdAsync(this.UserProps.IdCandidateProvider);
                        if (!invokedAfterApplicatioModalSubmit)
                        {
                            if (candidateProvider is not null)
                            {
                                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(candidateProvider.IdCandidate_Provider, "CandidateProvider");
                                if (concurrencyInfoValue == null)
                                {
                                    await this.AddEntityIdAsCurrentlyOpened(candidateProvider.IdCandidate_Provider, "CandidateProvider");
                                }

                                await this.applicationModal.OpenModal(candidateProvider, true, false, concurrencyInfoValue);
                            }
                        }

                        this.applicationSource.Clear();
                        this.applicationSource.Add(candidateProvider);

                        this.title = candidateProvider is not null ? $"Профил на {candidateProvider.CPONameAndOwner}" : string.Empty;
                    }
                    else if (entryFrom == GlobalConstants.TOKEN_CIPO_PROFILE_VALUE)
                    {
                        this.isCPO = false;

                        var candidateProvider = await this.CandidateProviderService.GetActiveCandidateProviderWithLocationIncludedByIdAsync(this.UserProps.IdCandidateProvider);
                        if (!invokedAfterApplicatioModalSubmit)
                        {
                            if (candidateProvider is not null)
                            {
                                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(candidateProvider.IdCandidate_Provider, "CandidateProvider");
                                if (concurrencyInfoValue == null)
                                {
                                    await this.AddEntityIdAsCurrentlyOpened(candidateProvider.IdCandidate_Provider, "CandidateProvider");
                                }

                                await this.applicationModal.OpenModal(candidateProvider, true, false, concurrencyInfoValue);
                            }
                        }

                        this.applicationSource.Clear();
                        this.applicationSource.Add(candidateProvider);

                        this.title = candidateProvider is not null ? $"Профил на {candidateProvider.CIPONameAndOwner}" : string.Empty;
                    }

                    await this.applicationGrid.Refresh();
                    this.StateHasChanged();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private async Task EditProfileBtn(CandidateProviderVM candidateProviderVM)
        {
            bool hasPermission = await this.CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(candidateProviderVM.IdCandidate_Provider, "CandidateProvider");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(candidateProviderVM.IdCandidate_Provider, "CandidateProvider");
                }

                await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = candidateProviderVM.IdCandidate_Provider }, true, false, concurrencyInfoValue);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterProfileModalSubmitAsync()
        {
            await this.HandleTokenData(true);
        }
    }
}
