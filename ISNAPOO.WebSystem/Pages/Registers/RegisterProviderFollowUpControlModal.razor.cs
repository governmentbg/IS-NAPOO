using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Registers
{
    public partial class RegisterProviderFollowUpControlModal : BlazorBaseComponent
    {
        private IEnumerable<FollowUpControlVM> followUpControlSource = new List<FollowUpControlVM>();
        private SfGrid<FollowUpControlVM> sfGrid = new SfGrid<FollowUpControlVM>();
        private SfDialog sfDialog = new SfDialog();
        private FollowUpControlReadonlyModal followUpControlModal = new FollowUpControlReadonlyModal();
        private string LicensingType = "LicensingCPO";

        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public IControlService ControlService { get; set; }

        public async Task OpenModal(CandidateProviderVM candidateProviderVM)
        {

            this.isVisible = true;

            followUpControlSource = new List<FollowUpControlVM>();
            followUpControlSource = candidateProviderVM.FollowUpControls.OrderByDescending(x =>x.ControlEndDate);

            var kvFollowUpControlTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlType");
            var kvControlStatusesSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlStatus");
            var kvControlTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ControlType");

            foreach (var item in followUpControlSource)
            {
                item.FollowUpControlTypeName = kvFollowUpControlTypeSource.FirstOrDefault(c => c.IdKeyValue == item.IdFollowUpControlType).Name;
                item.ControlTypeName = kvControlTypeSource.FirstOrDefault(c => c.IdKeyValue == item.IdControlType).Name;
                item.StatusName = kvControlStatusesSource.FirstOrDefault(c => c.IdKeyValue == item.IdStatus.Value).Name;
            }

            this.StateHasChanged();
        }
        private async Task OpenFollowUpControl(FollowUpControlVM followUpControl)
        {
            var model = await this.ControlService.GetControlByIdFollowUpControlAsync(followUpControl.IdFollowUpControl);
            this.LicensingType = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType")).FirstOrDefault(x => x.IdKeyValue == model.CandidateProvider.IdTypeLicense).KeyValueIntCode;
            this.followUpControlModal.OpenModal(model);
        }
    }
}
