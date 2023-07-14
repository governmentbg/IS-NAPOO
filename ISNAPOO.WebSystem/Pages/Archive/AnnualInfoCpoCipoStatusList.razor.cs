using Data.Models.Data.Archive;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualInfoCpoCipoStatusList : BlazorBaseComponent
    {
        [Inject]
        public IArchiveService ArchiveService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        private AnnualInfoVM annualInfo = new AnnualInfoVM();
        private SfGrid<AnnualInfoStatusVM> statusesGrid = new SfGrid<AnnualInfoStatusVM>();
        private SfDialog sfDialog = new SfDialog();
        private bool showFileName = false;
        public async Task OpenModal(AnnualInfoVM annualInfoVM)
        {
            this.isVisible = true;
            this.annualInfo = annualInfoVM;
            this.showFileName = false;

            var kvCipoLicense = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");
            if (annualInfo.CandidateProvider.IdTypeLicense == kvCipoLicense.IdKeyValue)
            {
                this.showFileName = true;
            }
            
            foreach (var status in this.annualInfo.AnnualInfoStatuses)
            {
                var kvStatus = await this.DataSourceService.GetKeyValueByIdAsync(status.IdStatus);
                status.StatusValue = kvStatus.Name;
                status.StatusValueIntCode = kvStatus.KeyValueIntCode; ;

                status.StatusDate = status.ModifyDate;
                status.PersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(status.IdCreateUser);

            }
            this.StateHasChanged();
        }

        private async Task OnDownloadClick(AnnualInfoStatusVM annualInfoStatusVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<SelfAssessmentReportStatus>(annualInfoStatusVM.IdAnnualInfoStatus);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<SelfAssessmentReportStatus>(annualInfoStatusVM.IdAnnualInfoStatus);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, annualInfoStatusVM.FileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
