using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ImportCourseScheduleModal : BlazorBaseComponent
    {
        private SfUploader sfUpload = new SfUploader();

        private MemoryStream excelStream;
        private CourseScheduleVM courseScheduleVM = new CourseScheduleVM();
        private CourseVM courseVM = new CourseVM();

        [Parameter]
        public EventCallback CallbackAfterSave { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public async Task OpenModal(int idCourse)
        {
            await this.sfUpload.ClearAllAsync();

            this.courseScheduleVM = new CourseScheduleVM();

            this.courseVM = await this.TrainingService.GetTrainingCourseWithoutAnythingIncludedByIdAsync(idCourse);

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Cancel()
        {
            await this.sfUpload.ClearAllAsync();
            this.courseScheduleVM = new CourseScheduleVM();
            this.isVisible = false;
            this.StateHasChanged();
        }

        private async Task ImportCourseScheduleBtn()
        {
            bool isFileUploaded = !string.IsNullOrEmpty(this.courseScheduleVM.UploadedFileName) || this.courseScheduleVM.UploadedFileName != "#";
            if (!isFileUploaded)
            {
                await this.ShowErrorAsync("Моля, качете файл преди да импортирате!");
            }
            else
            {
                this.SpinnerShow();

                var resultContext = await this.TrainingService.ImportCourseScheduleAsync(this.courseScheduleVM.UploadedFileStream, this.courseScheduleVM.UploadedFileName, this.courseVM);
                await this.sfUpload.ClearAllAsync();

                if (resultContext.HasErrorMessages)
                {
                    this.excelStream = this.TrainingService.CreateCourseScheduleExcelWithErrors(resultContext);
                    await this.JsRuntime.SaveAs($"Greshki_import_dnevnik_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.excelStream.ToArray());

                    this.SpinnerHide();

                    await this.ShowErrorAsync("Неуспешен импорт! Файлът, който се опитвате да импортирате не отговаря на изискванията на шаблона!");
                }
                else
                {
                    await this.TrainingService.CreateCourseSchedulesFromListAsync(resultContext.ResultContextObject, this.courseVM.IdCourse);

                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                    this.courseScheduleVM = new CourseScheduleVM();
                    await this.CallbackAfterSave.InvokeAsync();

                    this.SpinnerHide();
                }

                this.isVisible = false;
            }
        }

        private void OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.courseScheduleVM.UploadedFileName = args.Files[0].FileInfo.Name;
                this.courseScheduleVM.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }
    }
}
