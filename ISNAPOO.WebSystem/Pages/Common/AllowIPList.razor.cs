using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Rating;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class AllowIPList : BlazorBaseComponent
    {

        [Inject]
        public IAllowIPService AllowIPService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        IEnumerable<AllowIPVM> allowIPVM;
        SfGrid<AllowIPVM> allowIPGrid = new SfGrid<AllowIPVM>();

        private AllowIPModal allowIPModal = new AllowIPModal();
        private AllowIPVM ipToDelete = new AllowIPVM();
        int idAllowIPToDelete = 0;

        protected override async Task OnInitializedAsync()
        {

        }

        private async Task LoadAllowIPsDataAsync()
        {
            this.allowIPVM = await this.AllowIPService.GetAllAllowIPsAsync();

            this.StateHasChanged();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await this.LoadAllowIPsDataAsync();
            }
        }
        public async Task openNewAllowIPModal()
        {
            AllowIPVM allowIP = new AllowIPVM();

            allowIPModal.openAllowIPModal(allowIP);
        }
        public async Task SelectAllowIP(AllowIPVM allowIP)
        {
            var ip = await AllowIPService.GetAllowIPAsync(allowIP.idAllowIP);

            await allowIPModal.openAllowIPModal(ip);
        }
        private async Task DeleteRowAllowIP(AllowIPVM allowIP)
        {
            this.ipToDelete = allowIP;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);

            if (isConfirmed)
            {
                var result = await this.AllowIPService.DeleteAllowIPdByIdAsync(this.ipToDelete.idAllowIP);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                    await this.LoadAllowIPsDataAsync();
                }
            }
        }
    }
}
