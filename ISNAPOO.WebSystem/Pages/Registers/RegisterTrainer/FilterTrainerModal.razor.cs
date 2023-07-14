using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterTrainer
{
    public partial class FilterTrainerModal : BlazorBaseComponent
    {
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();

        private IEnumerable<KeyValueVM> kvEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvContractTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvCandidateProviderTrainerStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvPracticeOrTheorySource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> filterDataTypeSource = new List<KeyValueVM>();
        private IEnumerable<ProfessionalDirectionVM> professionalDirectionsSource = new List<ProfessionalDirectionVM>();
        private IEnumerable<ProfessionVM> professionsSource = new List<ProfessionVM>();
        private IEnumerable<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private IEnumerable<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private bool alreadyRendered = false;
        private RegisterTrainerVM model = new RegisterTrainerVM();
        private bool isCPO = false;
        private int kvSPPOOActiveStatus = 0;

        [Parameter]
        public EventCallback<List<RegisterTrainerVM>> CallbackAfterSave { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(bool isCPO)
        {
            this.editContext = new EditContext(this.model);

            this.isCPO = isCPO;

            this.model.IsCPO = this.isCPO;

            if (!this.alreadyRendered)
            {
                this.kvSPPOOActiveStatus = this.DataSourceService.GetActiveStatusID();

                var licensingType = this.isCPO ? "LicensingCPO" : "LicensingCIPO";
                this.candidateProvidersSource = (await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete(licensingType)).ToList();

                this.kvEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
                this.kvContractTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ContractType");
                this.kvCandidateProviderTrainerStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus");
                this.kvPracticeOrTheorySource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
                this.filterDataTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FilterDataType");

                if (this.isCPO)
                {
                    this.professionalDirectionsSource = this.DataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
                    this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
                    this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
                }
            }

            this.alreadyRendered = true;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void ClearFilter()
        {
            this.model = new RegisterTrainerVM();
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    var filteredResults = await this.CandidateProviderService.GetCandidateProviderTrainersByFilterModelAsync(this.model);

                    await this.CallbackAfterSave.InvokeAsync(filteredResults.ToList());

                    this.isVisible = false;
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "ProviderOwner", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.candidateProvidersSource, query);
        }

        private void OnProfessionalDirectionSelectedHandler(ChangeEventArgs<int?, ProfessionalDirectionVM> args)
        {
            if (args.Value.HasValue)
            {
                this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == this.kvSPPOOActiveStatus && x.IdProfessionalDirection == args.Value.Value).OrderBy(x => x.Code).ToList();
                var idProfessions = this.professionsSource.Select(x => x.IdProfession).ToList();
                this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == this.kvSPPOOActiveStatus && idProfessions.Contains(x.IdProfession)).OrderBy(x => x.Code).ToList();
            }
            else
            {
                this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
                this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
            }

            this.StateHasChanged();
        }

        private void OnProfessionSelectedHandler(ChangeEventArgs<int?, ProfessionVM> args)
        {
            if (args.Value.HasValue)
            {
                this.professionalDirectionsSource = this.DataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdStatus == this.kvSPPOOActiveStatus && x.IdProfessionalDirection == args.ItemData.IdProfessionalDirection).OrderBy(x => x.Code).ToList();
                this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == this.kvSPPOOActiveStatus && x.IdProfession == args.Value.Value).OrderBy(x => x.Code).ToList();
            }
            else
            {
                this.professionalDirectionsSource = this.DataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
                this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
            }

            this.StateHasChanged();
        }

        private void OnSpecialitySelectedHandler(ChangeEventArgs<int?, SpecialityVM> args)
        {
            if (args.Value.HasValue)
            {
                this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == this.kvSPPOOActiveStatus && x.IdProfession == args.ItemData.IdProfession).OrderBy(x => x.Code).ToList();
                var idsProfessionalDirection = this.professionsSource.Select(x => x.IdProfessionalDirection).ToList();
                this.professionalDirectionsSource = this.DataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdStatus == this.kvSPPOOActiveStatus && idsProfessionalDirection.Contains(x.IdProfessionalDirection)).OrderBy(x => x.Code).ToList();
            }
            else
            {
                this.professionalDirectionsSource = this.DataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
                this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
            }

            this.StateHasChanged();
        }
    }
}
