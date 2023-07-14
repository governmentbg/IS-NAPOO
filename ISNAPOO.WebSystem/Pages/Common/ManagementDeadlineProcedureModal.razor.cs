using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class ManagementDeadlineProcedureModal : BlazorBaseComponent
    {
        ManagementDeadlineProcedureVM managementDeadlineProcedureVM = new ManagementDeadlineProcedureVM();
        SfDialog sfDialog;

        [Parameter]
        public EventCallback< ManagementDeadlineProcedureVM> CallbackAfterSave { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public IManagementDeadlineProcedureService ManagementDeadlineProcedureService { get; set; }

        private IEnumerable<KeyValueVM> kvLicensingTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();

        ResultContext<ManagementDeadlineProcedureVM> resultContext = new ResultContext<ManagementDeadlineProcedureVM>();

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.resultContext.ResultContextObject);

        }
        private async Task Save()
        {
            //bool hasPermission = await CheckUserActionPermission("ManageDocumentOfferData", false);
            //if (!hasPermission) { return; }


            
            this.SpinnerShow();
            this.editContext = new EditContext(this.managementDeadlineProcedureVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.isSubmitClicked = true;
            var result = 0;

            bool isValid = this.editContext.Validate();
            if (isValid)
            {

                this.resultContext.ResultContextObject = this.managementDeadlineProcedureVM;


                this.resultContext.ResultContextObject.Term = Int32.Parse(this.resultContext.ResultContextObject.TermAsStr);

                this.resultContext = await this.ManagementDeadlineProcedureService.SaveManagementDeadlineProcedureAsync(this.resultContext);

                await this.ShowSuccessAsync("Записът e успешeн!");
            }


            await CallbackAfterSave.InvokeAsync(this.managementDeadlineProcedureVM);

            this.SpinnerHide();
           
        }
        public async Task OpenModal(ManagementDeadlineProcedureVM _model)
        {
            managementDeadlineProcedureVM = _model;

            this.kvLicensingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType");
            this.kvApplicationStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");

            this.isVisible = true;
            this.StateHasChanged();

        }


    }
}
