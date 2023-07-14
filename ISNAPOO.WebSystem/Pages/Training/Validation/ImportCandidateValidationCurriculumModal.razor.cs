using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ImportCandidateValidationCurriculumModal : BlazorBaseComponent
    {
        SfDialog sfDialog;
        SfUploader sfUpload;

        ToastMsg toast;
        MemoryStream excelStream;
        CandidateCurriculumVM candidateCurriculumVM = new CandidateCurriculumVM();
        ValidationCurriculumVM validationCurriculumVM = new ValidationCurriculumVM();

        private int idCandidateProviderSpeciality = 0;
        private int idValidationClient = 0;
        private bool fileExist = false;

        [Parameter]
        public EventCallback<List<CandidateCurriculumVM>> CallbackAfterSave { get; set; }

        [Parameter]
        public EventCallback CallbackAfterValidationCurriculumImport { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateCurriculumService CandidateCurriculumService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public void OpenModal(int idCandidateProviderSpeciality = 0, int idValidationClient = 0)
        {
            if (idCandidateProviderSpeciality != 0)
            {
                this.idCandidateProviderSpeciality = idCandidateProviderSpeciality;
            }

            if (idValidationClient != 0)
            {
                this.idValidationClient = idValidationClient;
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void Cancel()
        {
            this.sfUpload.ClearAllAsync();
            this.candidateCurriculumVM = new CandidateCurriculumVM();
            this.validationCurriculumVM = new ValidationCurriculumVM();
            this.isVisible = false;
            this.StateHasChanged();
        }

        private async Task ImportCandidateCurriculumAync()
        {
            bool isFileUploaded = false;
            if (this.idCandidateProviderSpeciality != 0)
            {
                isFileUploaded = !string.IsNullOrEmpty(this.candidateCurriculumVM.UploadedFileName) || this.candidateCurriculumVM.UploadedFileName != "#";
            }
            else
            {
                isFileUploaded = !string.IsNullOrEmpty(this.validationCurriculumVM.UploadedFileName) || this.validationCurriculumVM.UploadedFileName != "#";
            }

            if (!isFileUploaded)
            {
                await this.ShowErrorAsync("Моля, качете файл преди да импортирате!");
            }
            else
            {
                if (this.idCandidateProviderSpeciality != 0)
                {
                    this.SpinnerShow();

                    var resultContext = await this.CandidateCurriculumService.ImportCurriculumAsync(this.candidateCurriculumVM.UploadedFileStream, this.candidateCurriculumVM.UploadedFileName, 0);
                    await this.sfUpload.ClearAllAsync();

                    if (resultContext.HasErrorMessages)
                    {
                        this.excelStream = this.CandidateCurriculumService.CreateExcelWithErrors(resultContext);
                        await this.JsRuntime.SaveAs($"Errors_ImportCurriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.excelStream.ToArray());

                        this.SpinnerHide();

                        await this.ShowErrorAsync("Неуспешен импорт! Файлът, който се опитвате да импортирате не отговаря на изискванията на шаблона!");
                    }
                    else
                    {
                        foreach (var curriculum in resultContext.ResultContextObject)
                        {
                            var inputContext = new ResultContext<CandidateCurriculumVM>();
                            curriculum.IdCandidateProviderSpeciality = this.idCandidateProviderSpeciality;
                            inputContext.ResultContextObject = curriculum;
                            var result = await this.CandidateCurriculumService.AddCandidateCurriculumAsync(inputContext, true, true);
                        }

                        this.SpinnerHide();

                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.candidateCurriculumVM = new CandidateCurriculumVM();
                        await this.CallbackAfterSave.InvokeAsync(resultContext.ResultContextObject);
                    }

                    this.isVisible = false;
                }
                    this.SpinnerShow();
                if (this.idValidationClient != 0)
                {
                    var resultContext = await this.TrainingService.ImportValidationCurriculumAsync(this.validationCurriculumVM.UploadedFileStream, this.validationCurriculumVM.UploadedFileName);
                    await this.sfUpload.ClearAllAsync();

                    if (resultContext.HasErrorMessages)
                    {
                        this.excelStream = this.TrainingService.CreateValidationExcelWithErrors(resultContext);
                        await this.JsRuntime.SaveAs($"Errors_ImportCurriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.excelStream.ToArray());

                        this.SpinnerHide();

                        await this.ShowErrorAsync("Неуспешен импорт! Файлът, който се опитвате да импортирате не отговаря на изискванията на шаблона!");
                    }
                    else
                    {
                        var client = await this.TrainingService.GetValidationClientByIdAsync(idValidationClient);
                        var idCandidateProviderSpeciality = await this.TrainingService.GetIdCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderAsync(client.IdSpeciality.Value, this.UserProps.IdCandidateProvider);
                        foreach (var curriculum in resultContext.ResultContextObject)
                        {
                            var inputContext = new ResultContext<ValidationCurriculumVM>();
                            curriculum.IdCandidateProviderSpeciality = idCandidateProviderSpeciality;
                            curriculum.IdValidationClient = idValidationClient;
                            inputContext.ResultContextObject = curriculum;
                            var result = await this.TrainingService.AddValidationCurriculumAsync(inputContext, true);
                        }

                        this.SpinnerHide();

                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.validationCurriculumVM = new ValidationCurriculumVM();
                        await this.CallbackAfterValidationCurriculumImport.InvokeAsync();
                    }

                    this.isVisible = false;
                }
            }

            this.StateHasChanged();
        }

        private void OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.candidateCurriculumVM.UploadedFileName = args.Files[0].FileInfo.Name;
                this.candidateCurriculumVM.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }

        private void OnChangeValidationCurriculum(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.fileExist = true;
                this.validationCurriculumVM.UploadedFileName = args.Files[0].FileInfo.Name;
                this.validationCurriculumVM.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }
        private void RemoveFileValidationCurriculum(RemovingEventArgs args)
        {
                this.fileExist = false;
                this.sfUpload.ClearAllAsync();           
                this.StateHasChanged();           
        }
        private void ClearFileValidationCurriculum(ClearingEventArgs args)
        {
            this.fileExist = false;
           // this.sfUpload.ClearAllAsync();
            this.StateHasChanged();
        }
    }
}
