using DocuWorkService;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Control
{
    public partial class FollowUpControlReadonlyModal : BlazorBaseComponent
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

        private int selectedTab = 0;
        private FollowUpControlDocumentsList followUpControlDocumentsList = new FollowUpControlDocumentsList();
        private FollowUpControlInformation followUpControlInformation = new FollowUpControlInformation();
        private CheckingObjectReadOnly checkingObjectReadOnly = new CheckingObjectReadOnly();
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;
        private string CPOorCIPONameAndOwner = string.Empty;
        private bool IsCPO = true;

        private FollowUpControlVM model = new FollowUpControlVM();

        [Parameter]
        public string LicenseType { get; set; }

        [Parameter]
        public bool IsEditable { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);

            if (LicenseType == "LicensingCPO")
            {
                this.IsCPO = true;
                this.CPOorCIPONameAndOwner = this.model.IdFollowUpControl != 0 ? this.model.CandidateProvider.CPONameOwnerGrid : string.Empty;
            }
            else
            {
                this.IsCPO = false;
                this.CPOorCIPONameAndOwner = this.model.IdFollowUpControl != 0 ? this.model.CandidateProvider.CIPONameOwnerGrid : string.Empty;
            }
        }

        public async Task OpenModal(FollowUpControlVM _model)
        {
            this.selectedTab = 0;

            if (_model.IdFollowUpControl != 0)
            {
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
        }

        //private async Task SelectedEventHandler()
        //{
        //    if (this.selectedTab == 0)
        //    {
        //        await this.followUpControlInformation.OpenModal(this.model);
        //    }
        //    else if (this.selectedTab == 1)
        //    {
        //        await this.checkingObjectReadOnly.OpenModal(this.model);
        //    }
        //    else if (this.selectedTab == 2)
        //    {
        //        await this.followUpControlDocumentsList.OpenModal(this.model);
        //    }
        //}

        private void CloseModal()
        {
            this.isVisible = false;
        }
    }
}
