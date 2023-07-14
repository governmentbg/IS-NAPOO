using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseClientsImportModal : BlazorBaseComponent
    {
        private SfUploader sfUpload = new SfUploader();

        private ClientCourseVM clientCourseVM = new ClientCourseVM();
        private MemoryStream excelStream;
        private CourseVM course = new CourseVM();
        private List<ClientCourseVM> addedClients = new List<ClientCourseVM>();

        [Parameter]
        public EventCallback CallbackAfterImport { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public async Task OpenModal(CourseVM course, List<ClientCourseVM> addedClients)
        {
            this.clientCourseVM = new ClientCourseVM();

            await this.sfUpload.ClearAllAsync();

            this.course = course;
            this.addedClients = addedClients;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task ImportCourseClientsAync()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (string.IsNullOrEmpty(this.clientCourseVM.UploadedFileName) || this.clientCourseVM.UploadedFileName == "#")
                {
                    await this.ShowErrorAsync("Моля, качете файл преди да импортирате!");
                }
                else
                {
                    this.SpinnerShow();

                    var resultContext = await this.TrainingService.ImportCourseClientsAsync(this.clientCourseVM.UploadedFileStream, this.clientCourseVM.UploadedFileName, this.course, this.addedClients);
                    await this.sfUpload.ClearAllAsync();

                    if (resultContext.HasErrorMessages)
                    {
                        this.excelStream = this.TrainingService.CreateClientCourseExcelWithErrors(resultContext);
                        await this.JsRuntime.SaveAs($"Errors_ImportClients_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.excelStream.ToArray());

                        this.SpinnerHide();

                        await this.ShowErrorAsync("Попълнената информация за курсисти не отговаря на изискванията! Моля, отстранете грешките във файла! Импортирането е неуспешно!"); 
                    }
                    else
                    {
                        foreach (var clientCourse in resultContext.ResultContextObject)
                        {
                            var inputContext = new ResultContext<ClientCourseVM>();
                            inputContext.ResultContextObject = clientCourse;
                            var result = await this.TrainingService.CreateTrainingClientCourseAsync(inputContext, this.UserProps.IdCandidateProvider, this.course.IdTrainingCourseType.Value);
                        }

                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.clientCourseVM = new ClientCourseVM();

                        await this.CallbackAfterImport.InvokeAsync();

                        this.SpinnerHide();
                    }

                    this.isVisible = false;
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.clientCourseVM.UploadedFileName = args.Files[0].FileInfo.Name;
                this.clientCourseVM.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }

        private void RemoveFile()
        {
            this.clientCourseVM = new ClientCourseVM();
        }
    }
}
