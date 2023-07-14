using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class NAPOODocumentFabricNumbersSearchModal : BlazorBaseComponent
    {
        private ToastMsg toast = new ToastMsg();
        private SfAutoComplete<int, CandidateProviderVM> providersAutoComplete = new SfAutoComplete<int, CandidateProviderVM>();

        private NAPOODocumentSerialNumberFilterVM napooDocumentSerialNumberFilterVM = new NAPOODocumentSerialNumberFilterVM();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentSource = new List<TypeOfRequestedDocumentVM>();
        private List<CandidateProviderVM> providersSource = new List<CandidateProviderVM>();
        private IEnumerable<KeyValueVM> kvDocumentOperationsSource = new List<KeyValueVM>();
        private bool isCPO = false;

        [Parameter]
        public EventCallback<List<DocumentSerialNumberVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.napooDocumentSerialNumberFilterVM);
        }

        public async Task OpenModal()
        {
            this.editContext = new EditContext(this.napooDocumentSerialNumberFilterVM);

            this.typeOfRequestedDocumentSource = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).Where(d => d.HasSerialNumber).ToList();
            this.providersSource = (await this.CandidateProviderService.GetAllActiveCandidateProvidersWithoutAnythingIncludedAsync(new CandidateProviderVM() { IsActive = true })).ToList();
            this.kvDocumentOperationsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");

            this.isCPO = !(await this.IsInRole("NAPOO_Expert"));
            if (this.isCPO)
            {
                this.napooDocumentSerialNumberFilterVM.IdCandidateProvider = this.UserProps.IdCandidateProvider;
            }
            else
            {
                this.napooDocumentSerialNumberFilterVM.IdCandidateProvider = 0;
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void ClearBtn()
        {
            this.napooDocumentSerialNumberFilterVM = new NAPOODocumentSerialNumberFilterVM();
        }

        private async Task SearchBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.napooDocumentSerialNumberFilterVM);
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    var documentSerialNumbers = (await this.ProviderDocumentRequestService.FilterDocumentSerialNumbersAsync(this.napooDocumentSerialNumberFilterVM)).ToList();

                    await this.CallbackAfterSubmit.InvokeAsync(documentSerialNumbers);

                    this.isVisible = false;
                }
            }
            finally
            {
                this.loading = false;
            }
            this.SpinnerHide();
            
        }

        private async Task OnFilterProvider(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                var query = new Query().Where(new WhereFilter() { Field = "CPONameOwnerAndBulstat", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.providersAutoComplete.FilterAsync(this.providersSource, query);
            }
        }
    }
}
