using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using ISNAPOO.Core.HelperClasses;

namespace ISNAPOO.WebSystem.Pages.Assessment
{
    public partial class ImportSurveyModal : BlazorBaseComponent
    {
        private SfUploader uploader = new SfUploader();

        private SurveyVM surveyVM = new SurveyVM();
        private MemoryStream excelStream;

        [Parameter]
        public EventCallback CallbackAfterImport { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        public async Task OpenModal(SurveyVM survey)
        {
            await this.uploader.ClearAllAsync();

            this.surveyVM = await this.AssessmentService.GetSurveyByIdAsync(survey.IdSurvey);

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.surveyVM.UploadedFileName = args.Files[0].FileInfo.Name;
                this.surveyVM.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }

        private void RemoveFile()
        {
            this.surveyVM = new SurveyVM();
        }

        private async Task ImportSurveyResultsBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (string.IsNullOrEmpty(this.surveyVM.UploadedFileName) || this.surveyVM.UploadedFileName == "#")
                {
                    await this.ShowErrorAsync("Моля, качете файл преди да импортирате!");
                }
                else
                {
                    var resultContext = await this.AssessmentService.ImportSurveyResultsAsync(this.surveyVM.UploadedFileStream, this.surveyVM.UploadedFileName, this.surveyVM);
                    await this.uploader.ClearAllAsync();

                    if (resultContext.HasErrorMessages)
                    {
                        this.excelStream = this.AssessmentService.CreateSurveyResultsExcelWithErrors(resultContext);
                        await this.JsRuntime.SaveAs($"Greshki_import_rezultati_anketa_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.excelStream.ToArray());

                        this.SpinnerHide();

                        await this.ShowErrorAsync("Попълнената информация за курсисти не отговаря на изискванията! Моля, отстранете грешките във файла! Импортирането е неуспешно!");
                    }
                    else
                    {
                        foreach (var surveyResult in resultContext.ResultContextObject)
                        {
                            surveyResult.EndDate = DateTime.Now;
                            var inputContext = new ResultContext<SurveyResultVM>();
                            inputContext.ResultContextObject = surveyResult;
                            var result = await this.AssessmentService.CreateSurveyResultFromExcelImportAsync(inputContext);
                        }

                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.surveyVM = new SurveyVM();

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
    }
}
