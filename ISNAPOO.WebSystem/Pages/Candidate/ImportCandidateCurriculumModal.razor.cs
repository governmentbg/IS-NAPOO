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

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ImportCandidateCurriculumModal : BlazorBaseComponent
    {
        SfDialog sfDialog;
        SfUploader sfUpload = new SfUploader();

        ToastMsg toast;
        MemoryStream excelStream;
        CandidateCurriculumVM candidateCurriculumVM = new CandidateCurriculumVM();
        TrainingCurriculumVM trainingCurriculumVM = new TrainingCurriculumVM();

        private int idCandidateProviderSpeciality = 0;
        private int idProgram = 0;
        private CandidateCurriculumModificationVM candidateCurriculumModificationVM = new CandidateCurriculumModificationVM();

        [Parameter]
        public EventCallback<List<CandidateCurriculumVM>> CallbackAfterSave { get; set; }

        [Parameter]
        public EventCallback CallbackAfterTrainingCurriculumImport { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateCurriculumService CandidateCurriculumService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public async Task OpenModal(int idCandidateProviderSpeciality, int idProgram)
        {
            this.idCandidateProviderSpeciality = idCandidateProviderSpeciality;
            if (this.idCandidateProviderSpeciality != 0)
            {
                this.candidateCurriculumModificationVM = await this.CandidateProviderService.GetCandidateCurriculumModificationWhenApplicationByIdCandidateProviderSpecialityAsync(this.idCandidateProviderSpeciality);
            }

            if (idProgram != 0)
            {
                this.idProgram = idProgram;
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Cancel()
        {
            await this.sfUpload.ClearAllAsync();
            this.candidateCurriculumVM = new CandidateCurriculumVM();
            this.trainingCurriculumVM = new TrainingCurriculumVM();
            this.isVisible = false;
            this.StateHasChanged();
        }

        private async Task ImportCandidateCurriculumAync()
        {
            bool isFileUploaded = false;
            if (this.candidateCurriculumModificationVM.IdCandidateProviderSpeciality != 0)
            {
                isFileUploaded = !string.IsNullOrEmpty(this.candidateCurriculumVM.UploadedFileName) || this.candidateCurriculumVM.UploadedFileName != "#";
            }
            else
            {
                isFileUploaded = !string.IsNullOrEmpty(this.trainingCurriculumVM.UploadedFileName) || this.trainingCurriculumVM.UploadedFileName != "#";
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

                    var resultContext = await this.CandidateCurriculumService.ImportCurriculumAsync(this.candidateCurriculumVM.UploadedFileStream, this.candidateCurriculumVM.UploadedFileName, this.candidateCurriculumModificationVM.IdCandidateCurriculumModification);
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
                            curriculum.IdCandidateProviderSpeciality = this.candidateCurriculumModificationVM.IdCandidateProviderSpeciality;
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

                if (this.idProgram != 0)
                {
                    this.SpinnerShow();

                    var resultContext = await this.TrainingService.ImportCurriculumAsync(this.trainingCurriculumVM.UploadedFileStream, this.trainingCurriculumVM.UploadedFileName);
                    await this.sfUpload.ClearAllAsync();

                    if (resultContext.HasErrorMessages)
                    {
                        this.excelStream = this.TrainingService.CreateExcelWithErrors(resultContext);
                        await this.JsRuntime.SaveAs($"Errors_ImportCurriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", this.excelStream.ToArray());

                        this.SpinnerHide();

                        await this.ShowErrorAsync("Неуспешен импорт! Файлът, който се опитвате да импортирате не отговаря на изискванията на шаблона!");
                    }
                    else
                    {
                        var program = await this.TrainingService.GetTrainingProgramByIdWithoutAnythingIncludedAsync(this.idProgram);
                        var idCandidateProviderSpeciality = await this.TrainingService.GetIdCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderAsync(program.IdSpeciality, this.UserProps.IdCandidateProvider);
                        foreach (var curriculum in resultContext.ResultContextObject)
                        {
                            var inputContext = new ResultContext<TrainingCurriculumVM>();
                            curriculum.IdProgram = this.idProgram;
                            curriculum.IdCandidateProviderSpeciality = idCandidateProviderSpeciality;
                            inputContext.ResultContextObject = curriculum;
                            var result = await this.TrainingService.AddTrainingCurriculumAsync(inputContext, true);
                        }

                        this.SpinnerHide();

                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.trainingCurriculumVM = new TrainingCurriculumVM();
                        await this.CallbackAfterTrainingCurriculumImport.InvokeAsync();
                    }

                    this.isVisible = false;
                }
            }
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

        private void OnChangeTrainingCurriculum(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                this.trainingCurriculumVM.UploadedFileName = args.Files[0].FileInfo.Name;
                this.trainingCurriculumVM.UploadedFileStream = args.Files[0].Stream;
                this.StateHasChanged();
            }
        }
    }
}
