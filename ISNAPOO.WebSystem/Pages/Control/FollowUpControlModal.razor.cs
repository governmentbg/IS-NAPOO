using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Candidate;
using Syncfusion.Blazor.Popups;
using ISNAPOO.WebSystem.Pages.Common;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Data;
using Syncfusion.DocIORenderer;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Microsoft.JSInterop;
using ISNAPOO.Core.HelperClasses;
using DocuServiceReference;
using DocuWorkService;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Core.Contracts.Training;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class FollowUpControlModal : BlazorBaseComponent
    {
        #region Inject
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public IControlService ControlService { get; set; }

        [Inject]
        public IExpertService ExpertService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDocuService DocuService { get; set; }

        [Inject]
        public ITemplateDocumentService TemplateDocumentService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }
        #endregion

        private SfDialog sfDialog = new SfDialog();
        private ToastMsg toast;
        private int selectedTab = 0;
        private FollowUpControlDocumentsList followUpControlDocumentsList = new FollowUpControlDocumentsList();
        private FollowUpControlInformation followUpControlInformation = new FollowUpControlInformation();
        private FollowUpControlUploadedFilesList followUpControlUploadedFilesList = new FollowUpControlUploadedFilesList();
        private CheckingObject checkingObject = new CheckingObject();
        private List<string> validationMessages = new List<string>();
        private ValidationMessageStore? messageStore;
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;
        private string CPOorCIPONameAndOwner = string.Empty;
        private string Header = string.Empty;
        private bool IsCPO = true;
        private bool isAllreadyOpen = false;


        private FollowUpControlVM model = new FollowUpControlVM();

        [Parameter]

        public EventCallback CallbackAfterSave { get; set; }

        [Parameter]

        public string LicenseType { get; set; }

        [Parameter]

        public bool IsEditable { get; set; } = true;

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);

            if (LicenseType == "LicensingCPO")
            {
                IsCPO = true;   
            }
            else
            {
                IsCPO = false;
            }
        }

        public async Task OpenModal(FollowUpControlVM _model)
        {
            this.validationMessages.Clear();
            selectedTab = 0;
            if (_model.IdFollowUpControl != 0)
            {
                if(IsCPO)
                {
                    this.CPOorCIPONameAndOwner = this.model.IdFollowUpControl != 0 ? this.model.CandidateProvider.CPONameOwnerGrid : string.Empty;
                }
                else
                {
                    this.CPOorCIPONameAndOwner = this.model.IdFollowUpControl != 0 ? this.model.CandidateProvider.CIPONameOwnerGrid : string.Empty;
                }
                
                this.model = _model;
                this.CreationDateStr = model.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = model.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(model.IdCreateUser);

            }
            else
            {
                this.model = new FollowUpControlVM();
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.model.ModifyPersonName = "";
                this.model.CreatePersonName = "";

            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
            isAllreadyOpen = true;
        }
        private async Task SelectedEventHandler()
        {
            if (this.selectedTab == 0)
            {
                await this.followUpControlInformation.LoadData();
            }
            else if (this.selectedTab == 1)
            {
                await this.checkingObject.LoadData();
            }
            else if (this.selectedTab == 2)
            {
                await this.followUpControlDocumentsList.LoadData();
            }
            else if (this.selectedTab == 3)
            {
                await this.followUpControlUploadedFilesList.LoadData();
            }
        }
        private async Task UpdateAfterSave(FollowUpControlVM _model)
        {
            this.model = await this.ControlService.GetControlByIdFollowUpControlAsync(_model.IdFollowUpControl);
            this.CreationDateStr = model.CreationDate.ToString("dd.MM.yyyy");
            this.ModifyDateStr = model.ModifyDate.ToString("dd.MM.yyyy");
            this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(model.IdModifyUser);
            this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(model.IdCreateUser);
        }

        private async Task Submit()
        {
            this.SpinnerShow();

            this.validationMessages.Clear();
            await this.followUpControlInformation.SubmitHandler();
            this.validationMessages.AddRange(this.followUpControlInformation.GetValidationMessages());
            if (!this.validationMessages.Any() && isAllreadyOpen)
            {
                await this.checkingObject.SubmitHandler();
            }

            if (!this.validationMessages.Any())
            {
                await this.CallbackAfterSave.InvokeAsync();
            }

            this.StateHasChanged();
            this.SpinnerHide();
        }

        private void ChangeHeader(string str)
        {
            this.Header = str;
            this.StateHasChanged();
        }


    }
}
