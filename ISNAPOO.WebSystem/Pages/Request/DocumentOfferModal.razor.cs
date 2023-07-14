using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request
{
    public partial class DocumentOfferModal : BlazorBaseComponent
    {
        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<ResultContext<ProviderDocumentOfferVM>> CallbackAfterSubmit { get; set; }

        #region Inject
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IProviderService providerService { get; set; }

        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }

        [Inject]
        public IProviderDocumentRequestService providerDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        #endregion



        private DialogEffect AnimationEffect = DialogEffect.Zoom;

        SfDialog sfDialog;
        ResultContext<ProviderDocumentOfferVM> resultContext = new ResultContext<ProviderDocumentOfferVM>();

        private string cpoName = string.Empty;
        private IEnumerable<KeyValueVM> kvOfferTypeSource = new List<KeyValueVM>();
        IEnumerable<CandidateProviderVM> candidateProviderSource;
        IEnumerable<TypeOfRequestedDocumentVM> typeRequestDocumentSource;

        public string CreationDateStr { get; set; } = "";
        public string ModifyDateStr { get; set; } = "";

        private bool IsDateValid = true;
        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.resultContext.ResultContextObject);
        }
        private void IsEndDateValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.resultContext.ResultContextObject.OfferStartDate.HasValue)
            {
                startDate = this.resultContext.ResultContextObject.OfferStartDate.Value.Date;
            }
            if (this.resultContext.ResultContextObject.OfferEndDate.HasValue)
            {
                endDate = this.resultContext.ResultContextObject.OfferEndDate.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);
            if (result > 0)
            {
                IsDateValid = false;
            }
            else
            {
                IsDateValid = true;
            }
        }

        public async Task OpenModal(ProviderDocumentOfferVM _model)
        {
            this.kvOfferTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("OfferType");
            this.candidateProviderSource = await this.candidateProviderService.GetAllActiveCandidateProvidersWithoutAnythingIncludedAsync(new CandidateProviderVM() { IsActive = true });
            this.typeRequestDocumentSource = await this.providerDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync();

            if (_model.CandidateProvider != null)
            {
                this.cpoName = "- " + _model.CandidateProvider.ProviderName + "/" + _model.CandidateProvider.ProviderOwner;
            }
            else
            {
                var candidateProvider = await this.candidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = this.UserProps.IdCandidateProvider });
                this.cpoName = "- " + candidateProvider.ProviderOwner;
            }

            this.resultContext.ResultContextObject = _model;
            this.resultContext.ResultContextObject.IdCandidateProvider = this.UserProps.IdCandidateProvider;
            this.editContext = new EditContext(this.resultContext.ResultContextObject);

            if (this.resultContext.ResultContextObject.IdProviderDocumentOffer == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.resultContext.ResultContextObject.ModifyPersonName = "";
                this.resultContext.ResultContextObject.CreatePersonName = "";
            }
            else
            {
                this.CreationDateStr = this.resultContext.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.resultContext.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                this.resultContext.ResultContextObject.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdModifyUser);
                this.resultContext.ResultContextObject.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdCreateUser);
            }
            
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Save()
        {
            bool hasPermission = await CheckUserActionPermission("ManageDocumentOfferData", false);
            if (!hasPermission) { return; }

            this.editContext = new EditContext(this.resultContext.ResultContextObject);
            this.editContext.EnableDataAnnotationsValidation();


            bool isValid = this.editContext.Validate();
            if (IsDateValid)
            {
                if (isValid)
                {
                    this.SpinnerShow();
                    this.resultContext = await this.providerDocumentRequestService.SaveProviderDocumentOfferAsync(this.resultContext);
                    this.CreationDateStr = this.resultContext.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
                    this.ModifyDateStr = this.resultContext.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                    this.resultContext.ResultContextObject.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdModifyUser);
                    this.resultContext.ResultContextObject.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdCreateUser);
                    this.editContext = new EditContext(this.resultContext.ResultContextObject);
                    await this.CallbackAfterSubmit.InvokeAsync(resultContext);
                    this.SpinnerHide();
                }
            }
            else if(this.resultContext.ResultContextObject.OfferEndDate != null)
            {
                resultContext.AddErrorMessage("Крайната дата не може да е преди началната!");
                await this.CallbackAfterSubmit.InvokeAsync(resultContext);
            }
        }

    }
}
