using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.WebSystem.Resources;
using Syncfusion.Blazor.Inputs;
using Data.Models.Data.Candidate;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class UploadCurriculumFileModal : BlazorBaseComponent
    {
        private SfUploader uploader = new SfUploader();

        private CandidateCurriculumModificationVM candidateCurriculumModificationVM = new CandidateCurriculumModificationVM();
        private string title = string.Empty;
        private int idSpeciality = 0;
        private bool showDeleteConfirmDialog;
        private string fileNameForDeletion;

        [Parameter]
        public EventCallback<List<string>> CallbackAfterSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        public async Task OpenModal(CandidateCurriculumModificationVM candidateCurriculumModification, int idSpeciality)
        {
            await this.uploader.ClearAllAsync();

            this.candidateCurriculumModificationVM = candidateCurriculumModification;

            this.idSpeciality = idSpeciality;

            this.SetTitle(idSpeciality);

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void SetTitle(int idSpeciality)
        {
            var speciality = this.DataSourceService.GetAllSpecialitiesList().FirstOrDefault(x => x.IdSpeciality == idSpeciality);
            if (speciality is not null)
            {
                this.title = $"Прикачване на файл към учебен план за специалност <span style=\"color: #ffffff;\">{speciality.CodeAndName}</span>";
            }
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.candidateCurriculumModificationVM.UploadedFileName))
            {
                this.fileNameForDeletion = args.FilesData[0].Name;
                ConfirmDeleteCallback();
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.candidateCurriculumModificationVM.UploadedFileName))
            {
                this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                this.fileNameForDeletion = fileName;
                this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
                
            }
        }

        public async void ConfirmDeleteCallback()
        {

            this.showDeleteConfirmDialog = false;

            var result = await this.UploadFileService.RemoveFileByIdAsync<CandidateCurriculumModification>(this.candidateCurriculumModificationVM.IdCandidateCurriculumModification);
            if (result == 1)
            {
                this.candidateCurriculumModificationVM.UploadedFileName = null;

                await this.CallbackAfterSubmit.InvokeAsync(new List<string>()
                {
                    this.candidateCurriculumModificationVM.UploadedFileName,
                    this.idSpeciality.ToString()
                });
            }

            this.StateHasChanged();
        }

        private async Task OnDownloadClick()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.candidateCurriculumModificationVM.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateCurriculumModification>(this.candidateCurriculumModificationVM.IdCandidateCurriculumModification);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateCurriculumModification>(this.candidateCurriculumModificationVM.IdCandidateCurriculumModification);
                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.candidateCurriculumModificationVM.FileName, documentStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                        await this.ShowErrorAsync(msg);
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;
            var result = await this.UploadFileService.UploadFileAsync<CandidateCurriculumModification>(file, args.Files[0].FileInfo.Name, "CurriculumModification", this.candidateCurriculumModificationVM.IdCandidateCurriculumModification);
            if (!string.IsNullOrEmpty(result))
            {
                this.candidateCurriculumModificationVM.UploadedFileName = result;

                await this.CallbackAfterSubmit.InvokeAsync(new List<string>()
                {
                    this.candidateCurriculumModificationVM.UploadedFileName,
                    this.idSpeciality.ToString()
                });
            }

            this.StateHasChanged();
        }
    }
}
