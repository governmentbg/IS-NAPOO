using Data.Models.Data.Training;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class UploadCourseCurriculumFileModal : BlazorBaseComponent
    {
        private SfUploader uploader = new SfUploader();

        private bool isCourse = false;
        private int idEntity = 0;
        private string title = string.Empty;
        private string uploadedFileName = string.Empty;
        private string fileName = string.Empty;

        [Parameter]
        public EventCallback<string> CallbackAfterSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public async Task OpenModal(int idEntity, string title, bool isCourse)
        {
            await this.uploader.ClearAllAsync();
            this.uploadedFileName = string.Empty;
            this.fileName = string.Empty;

            this.isCourse = isCourse;
            this.idEntity = idEntity;

            this.title = this.isCourse
                ? $"Прикачване на файл с уч. програма за курс <span style=\"color: #ffffff;\">{title}</span>"
                : $"Прикачване на файл с уч. програма за валидирано лице <span style=\"color: #ffffff;\">{title}</span>";

            this.isVisible = true;
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
                if (this.isCourse)
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<Course>(this.idEntity);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<Course>(this.idEntity);

                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.fileName, documentStream.MS!.ToArray());
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
                    var documentStream = await this.UploadFileService.GetValidationClientCurriculumUploadedFileAsync(this.idEntity);

                    await FileUtils.SaveAs(this.JsRuntime, this.fileName, documentStream.ToArray());
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
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var fileStream = args.Files[0].Stream;
                this.fileName = args.Files[0].FileInfo.Name;
                if (this.isCourse)
                {
                    await this.UploadFileService.UploadFileAsync<Course>(fileStream, this.fileName, "Course", this.idEntity);
                    var course = await this.TrainingService.GetTrainingCourseWithoutAnythingIncludedByIdAsync(this.idEntity);
                    if (course is not null)
                    {
                        this.uploadedFileName = course.UploadedFileName;
                    }
                }
                else
                {
                    await this.UploadFileService.UploadFileAsync<ValidationClient>(fileStream, this.fileName, "ValidationClient", this.idEntity);
                    var validationClient = await this.TrainingService.GetValidationClientWithoutIncludesByIdAsync(this.idEntity);
                    if (validationClient is not null)
                    {
                        this.uploadedFileName = validationClient.UploadedCurriculumFileName;
                    }
                }
                 
                await this.CallbackAfterSubmit.InvokeAsync(this.uploadedFileName);

                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    if (this.isCourse)
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<Course>(this.idEntity);
                        if (result == 1)
                        {
                            this.uploadedFileName = string.Empty;
                        }
                    }
                    else
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationClient>(this.idEntity);
                        if (result == 1)
                        {
                            this.uploadedFileName = string.Empty;
                        }
                    }

                    this.StateHasChanged();
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OnRemove()
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    if (this.isCourse)
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<Course>(this.idEntity);
                        if (result == 1)
                        {
                            this.uploadedFileName = string.Empty;
                        }
                    }
                    else
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationClient>(this.idEntity);
                        if (result == 1)
                        {
                            this.uploadedFileName = string.Empty;
                        }
                    }

                    this.StateHasChanged();
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }
    }
}
