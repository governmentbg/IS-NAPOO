using Data.Models.Data.Archive;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentReportMainData : BlazorBaseComponent
    {
        [Parameter]
        public SelfAssessmentReportVM SelfAssessmentReportVM { get; set; }

        [Parameter]
        public bool DisableFields { get; set; }

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

        private SfGrid<SelfAssessmentReportStatusVM> statusesGrid = new SfGrid<SelfAssessmentReportStatusVM>();

         

        protected override async Task OnInitializedAsync()
        {

            this.FormTitle = "Основни данни";
            this.editContext = new EditContext(this.SelfAssessmentReportVM);

            RealoadStatusGrid();
        }

        public async Task RealoadStatusGrid() 
        {
            foreach (var status in SelfAssessmentReportVM.SelfAssessmentReportStatuses)
            {
                var kvStatus = await this.DataSourceService.GetKeyValueByIdAsync(status.IdStatus);
                status.StatusValue = kvStatus.Name;
                status.StatusValueIntCode = kvStatus.KeyValueIntCode; 

                status.StatusDate = status.ModifyDate;
                status.PersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(status.IdCreateUser);

                var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var fileFullName = settingResource + "\\UploadedFiles\\SelfAssessmentReportStatus\\" + SelfAssessmentReportVM.IdSelfAssessmentReport;
                //var fileFullName = settingResource + status.UploadedFileName;
                if (Directory.Exists(fileFullName))
                {
                    //\UploadedFiles\SelfAssessmentReportStatus\99\годишен доклад 2016.pdf
                    var files = Directory.GetFiles(fileFullName);
                    files = files.Select(x => x.Split(($"\\{SelfAssessmentReportVM.IdSelfAssessmentReport}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                    status.FileName = string.Join(Environment.NewLine, files);
                }

            }
            this.StateHasChanged();

        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
             
        }

        private async Task OnDownloadClick( SelfAssessmentReportStatusVM entity)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<SelfAssessmentReportStatus>(entity.IdSelfAssessmentReportStatus);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<SelfAssessmentReportStatus>(entity.IdSelfAssessmentReportStatus);
                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, entity.FileName, documentStream.MS!.ToArray());
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

        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.SelfAssessmentReportVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.editContext.Validate();
        }
    }
}
