using System.Collections.Generic;
using Blazored.LocalStorage;
using Data.Models.Data.ProviderData;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class ManagementDeadlineProcedureList : BlazorBaseComponent
    {


        [Inject]
        public IManagementDeadlineProcedureService ManagementDeadlineProcedureService { get; set; }

        private IEnumerable<ManagementDeadlineProcedureVM> dataSource = new List<ManagementDeadlineProcedureVM>();
        private SfGrid<ManagementDeadlineProcedureVM> sfGrid = new SfGrid<ManagementDeadlineProcedureVM>();

        private ManagementDeadlineProcedureModal managementDeadlineProcedureModal = new ManagementDeadlineProcedureModal();



        protected override async Task OnInitializedAsync()
        {
            dataSource = await ManagementDeadlineProcedureService.GetAllManagementDeadlineProceduresAsync();

            StateHasChanged();
        }



        private async Task UpdateAfterSave(ManagementDeadlineProcedureVM _model)
        {

            dataSource = await ManagementDeadlineProcedureService.GetAllManagementDeadlineProceduresAsync();

            StateHasChanged();

        }


        private async Task SelectedRow(ManagementDeadlineProcedureVM model)
        {
            //bool hasPermission = await CheckUserActionPermission("ViewSettingsData", false);
            //if (!hasPermission) { return; }

            await this.managementDeadlineProcedureModal.OpenModal(model);
        }



        private async Task OpenAddNewModal()
        {
            //bool hasPermission = await CheckUserActionPermission("ManageDocumentOfferData", false);
            //if (!hasPermission) { return; }

            //var model = new ProviderDocumentOfferVM();
            ManagementDeadlineProcedureVM model = new ManagementDeadlineProcedureVM();
            await this.managementDeadlineProcedureModal.OpenModal(model);
        }



    }
}
