using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.ExternalExpertCommission.Modals
{
    public partial class ParticipationCommisionModal : BlazorBaseComponent
    {
        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSave { get; set; }

        #region Inject
        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IExpertService expertService { get; set; }
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        #endregion


        private string dialogClass = "";
        private string CreationDate = "";
        private string ModifyDate = "";
        ToastMsg toast;
        ResultContext<ExpertExpertCommissionVM> resultContext = new ResultContext<ExpertExpertCommissionVM>();
        PersonVM Person = new PersonVM();


        private IEnumerable<KeyValueVM> expertCommissionValues = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> roleCommissionValues = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> statusValues = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvMemberTypeECSource = new List<KeyValueVM>();

        protected override async Task OnInitializedAsync()
        {
            this.resultContext.ResultContextObject = new ExpertExpertCommissionVM();
            editContext = new EditContext(this.resultContext.ResultContextObject);

            this.roleCommissionValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertRoleCommission");
            this.statusValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus");
        }

        public async Task OpenModal(ExpertExpertCommissionVM _model)
        {
            this.resultContext = new ResultContext<ExpertExpertCommissionVM>();
            this.expertCommissionValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");
            this.kvMemberTypeECSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MemberTypeECHolderAlternate");
            var expertCommissions = expertService.GetExpertExpertCommissionByExpertIdAsync(_model.IdExpert).Result;
            foreach (var item in expertCommissions)
            {
                if (item.IdKeyValue != _model.IdExpertCommission)
                {
                    expertCommissionValues = expertCommissionValues.Where(p => p.IdKeyValue != item.IdKeyValue).ToList();
                }
            }
            var expert = await expertService.GetExpertByIdAsync(_model.IdExpert);
            this.Person = expert.Person;
            if (_model.IdStatus == 0)
            {
                _model.IdStatus = this.statusValues.FirstOrDefault(s => s.KeyValueIntCode == "Active").IdKeyValue;
            }

            this.resultContext.ResultContextObject = _model;
            if (this.resultContext.ResultContextObject.IdExpertExpertCommission == 0)
            {
                this.CreationDate = "";
                this.ModifyDate = "";
                this.resultContext.ResultContextObject.CreatePersonName = "";
                this.resultContext.ResultContextObject.ModifyPersonName = "";

            }
            else
            {
                this.CreationDate = this.resultContext.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDate = this.resultContext.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                this.resultContext.ResultContextObject.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdCreateUser);
                this.resultContext.ResultContextObject.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdModifyUser);
            }
            this.editContext = new EditContext(this.resultContext.ResultContextObject);

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Save()
        {

            if (this.loading)
            {
                return;
            }

            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ManageExpertsData", false);
                if (!hasPermission) { return; }
                this.SpinnerShow();
                this.editContext = new EditContext(this.resultContext.ResultContextObject);
                this.editContext.EnableDataAnnotationsValidation();

                this.isSubmitClicked = true;


                bool isValid = this.editContext.Validate();
                if (isValid)
                {
                    this.resultContext = await this.expertService.SaveExpertExpertCommissionAsync(this.resultContext);

                    if (this.resultContext.HasMessages)
                    {
                        this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                        this.resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                        this.resultContext.ListErrorMessages.Clear();
                    }

                    this.CreationDate = this.resultContext.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
                    this.ModifyDate = this.resultContext.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                    this.resultContext.ResultContextObject.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdCreateUser);
                    this.resultContext.ResultContextObject.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdModifyUser);
                    await CallbackAfterSave.InvokeAsync();
                }

                this.isSubmitClicked = false;
                
            }
            finally
            {
                this.SpinnerHide();
                this.loading = false;
            }
        }


    }
}
