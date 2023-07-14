using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Core.XML.Course;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseImportModal : BlazorBaseComponent
    {
        private SfUploader sfUpload = new SfUploader();

        private MemoryStream excelStream;
        private CourseVM courseVM = new CourseVM();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public void OpenModal()
        {
            this.isVisible = true;
            this.StateHasChanged();
        }

        private void Cancel()
        {
            this.sfUpload.ClearAllAsync();
            this.courseVM = new CourseVM();
            this.isVisible = false;
            this.StateHasChanged();
        }

        private void OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.courseVM.UploadedFileName = args.Files[0].FileInfo.Name;
                this.courseVM.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }

        private async Task ImportCourseBtn()
        {
            bool isFileUploaded = !string.IsNullOrEmpty(this.courseVM.UploadedFileName) || this.courseVM.UploadedFileName != "#";
            if (!isFileUploaded)
            {
                await this.ShowErrorAsync("Моля, качете файл преди да импортирате!");
            }
            else
            {
                this.SpinnerShow();

                var resultContext = await this.TrainingService.ImportCourseAsync(this.courseVM.UploadedFileStream, this.courseVM.UploadedFileName);
                await this.sfUpload.ClearAllAsync();

                if (resultContext.HasErrorMessages)
                {
                    this.excelStream = this.TrainingService.CreateExcelWithXMLImportValidationErrors(resultContext);
                    await this.JsRuntime.SaveAs($"Errors_ImportCourse_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.excelStream.ToArray());

                    this.SpinnerHide();

                    await this.ShowErrorAsync("Неуспешен импорт! Файлът, който се опитвате да импортирате, не отговаря на изискванията на шаблона!");
                }
                else
                {
                    var resultFromImportIntoDB = new ResultContext<CourseCollection>();
                    resultFromImportIntoDB.ResultContextObject = resultContext.ResultContextObject;
                    resultFromImportIntoDB = await this.TrainingService.ImportXMLCourseIntoDBAsync(resultFromImportIntoDB);
                    if (resultFromImportIntoDB.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultFromImportIntoDB.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultFromImportIntoDB.ListMessages));

                        this.courseVM = new CourseVM();
                        await this.CallbackAfterSubmit.InvokeAsync();
                    }

                    this.SpinnerHide();
                }

                this.isVisible = false;
            }

            this.StateHasChanged();
        }
    }
}
