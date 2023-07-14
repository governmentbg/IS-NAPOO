using System.Data;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Common.UserManagement;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Common.ManagingTimelines
{
    public partial class TimelinesManagementList
    {
        public int RowCounter = 0;

        private SfGrid<ProcedureTimeline> refGrid;

        private ToastMsg toast = new ToastMsg();

        private TimelinesManagementModal roleModal = new TimelinesManagementModal();
        private int row = 1;

        protected override async Task OnInitializedAsync()
        {
            ProcedureTimelines = Enumerable.Range(1, 5).Select(x => new ProcedureTimeline()
            {
                ID = x.ToString(),
                LicensingType = "",
                ApplicationStatus = (new string[] { "ALFKI", "ANANTR", "ANTON", "BLONP", "BOLID" })[new Random().Next(5)],
                Period = DateTime.Now
            }).ToList();
        }

        public async Task<int> GetRowCounter(ProcedureTimeline val)
        {
            RowCounter = ProcedureTimelines.IndexOf(val);
            return RowCounter + 1;
        }
        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageRolesData", false);
            if (!hasPermission) { return; }

            await this.roleModal.OpenModal(new ProcedureTimeline());
        }

        private async Task SelectedRow(ProcedureTimeline _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewRolesData", false);
            if (!hasPermission) { return; }

            // await this.roleModal.OpenModal(_model);
        }

        public List<ProcedureTimeline> ProcedureTimelines { get; set; }

        public class ProcedureTimeline
        {
            public string ID { get; set; }
            public string LicensingType { get; set; }
            public string ApplicationStatus { get; set; }
            public DateTime Period { get; set; }
        }

        private async Task OnApplicationSubmit(ResultContext<ApplicationRoleVM> resultContext)
        {
            var msg = resultContext.ListMessages.FirstOrDefault();

            if (resultContext.ListMessages.Count() == 0)
            {
                this.toast.sfSuccessToast.Content = "Записът е успешен!";
                await this.toast.sfSuccessToast.ShowAsync();
                // var rolesSource = await ApplicationUserService.GetAllApplicationRoleAsync(new ApplicationRoleVM());
                // this.roles = rolesSource.OrderBy(v => v.Name).ToList();
            }
            else
            {
                var errorMsg = resultContext.ListErrorMessages.FirstOrDefault();
                this.toast.sfErrorToast.Content = msg + Environment.NewLine + errorMsg;
                await this.toast.sfErrorToast.ShowAsync();
            }
        }

        public async Task<string> GetRowNumber(string key)
        {
            var page = refGrid.PageSettings.CurrentPage - 1;
            var pageLength = refGrid.PageSettings.PageSize;
            var index = await refGrid.GetRowIndexByPrimaryKeyAsync(key);
            var num = page * pageLength + index;

            return $"{num + 1}.";
        }
    }
}
