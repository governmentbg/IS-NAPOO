using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOCandidateProviderTrainerDocumentModal : BlazorBaseComponent
    {
        SfDialog sfDialog = new SfDialog();
        private SfUploader uploader = new SfUploader();

        private CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM = new CandidateProviderTrainerDocumentVM();
        private IEnumerable<KeyValueVM> kvDocumentTypeSource = new List<KeyValueVM>();

        [Parameter]
        public EventCallback<CandidateProviderTrainerDocumentVM> CallbackAfterModalSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.candidateProviderTrainerDocumentVM);
        }

        private async Task SubmitDocumentHandler()
        {
            this.SpinnerShow();

            this.editContext = new EditContext(this.candidateProviderTrainerDocumentVM);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {
                ResultContext<CandidateProviderTrainerDocumentVM> resultContext = new ResultContext<CandidateProviderTrainerDocumentVM>();

                if (this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument != 0)
                {
                    resultContext = await this.CandidateProviderService.UpdateCandidateProviderTrainerDocumentAsync(this.candidateProviderTrainerDocumentVM);
                }
                else
                {
                    resultContext = await this.CandidateProviderService.CreateCandidateProviderTrainerDocumentAsync(this.candidateProviderTrainerDocumentVM);
                }

                if (resultContext.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));

                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                }

                await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderTrainerDocumentVM);
            }

            this.SpinnerHide();
        }

        public async Task OpenModal(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM, IEnumerable<KeyValueVM> kvDocumentTypeSource)
        {
            if (candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument != 0)
            {
                this.candidateProviderTrainerDocumentVM = await this.CandidateProviderService.GetCandidateProviderTrainerDocumentByIdAsync(candidateProviderTrainerDocumentVM);
            }
            else
            {
                this.candidateProviderTrainerDocumentVM = new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainer = candidateProviderTrainerDocumentVM.IdCandidateProviderTrainer };
            }

            this.kvDocumentTypeSource = kvDocumentTypeSource;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task OnRemoveClick(string fileName)
        {
            if (this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument > 0)
            {
                string msg = "Сигурни ли си сте, че искате да изтриете прикачения файл?";
                bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAndFileNameAsync<CandidateProviderTrainerDocument>(this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument, fileName);

                    if (result == 1)
                    {
                        this.candidateProviderTrainerDocumentVM = await this.CandidateProviderService.GetCandidateProviderTrainerDocumentByIdAsync(this.candidateProviderTrainerDocumentVM);
                        await this.SetFileNameAsync();

                        this.StateHasChanged();

                        await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderTrainerDocumentVM);
                    }
                }
            }
        }

        private async Task OnDownloadClick(string fileName)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderTrainerDocument>(this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderTrainerDocument>(this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument, fileName);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.MS!.ToArray());
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

        private async Task OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count == 1)
            {
                if (this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument > 0)
                {
                    bool isConfirmed = true;

                    if (isConfirmed)
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAndFileNameAsync<CandidateProviderTrainerDocument>(this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument, args.FilesData[0].Name);

                        if (result == 1)
                        {
                            this.candidateProviderTrainerDocumentVM = await this.CandidateProviderService.GetCandidateProviderTrainerDocumentByIdAsync(this.candidateProviderTrainerDocumentVM);

                            this.StateHasChanged();
                        }
                    }
                }
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count >= 1)
            {
                await this.SetNameForFilesWithSameFileNameAsync(args);

                var fileNames = args.Files.Select(x => $"{x.FileInfo.Name} ");
                var fileName = string.Join(Environment.NewLine, fileNames).Trim();
                var fileStreams = args.Files.Select(x => x.Stream).ToArray();

                var result = await this.UploadFileService.UploadFileCandidateProviderTrainerDocumentAsync(fileStreams, fileName, "CandidateProviderTrainerDocument", this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument);

                this.candidateProviderTrainerDocumentVM.UploadedFileName = result.ResultContextObject.UploadedFileName;

                await this.SetFileNameAsync();

                this.StateHasChanged();

                this.editContext = new EditContext(this.candidateProviderTrainerDocumentVM);
            }
        }

        private async Task SetNameForFilesWithSameFileNameAsync(UploadChangeEventArgs args)
        {
            var settingFolder = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            foreach (var file in args.Files)
            {
                if (!string.IsNullOrEmpty(this.candidateProviderTrainerDocumentVM.UploadedFileName))
                {
                    var path = settingFolder + "\\" + this.candidateProviderTrainerDocumentVM.UploadedFileName;
                    var files = Directory.GetFiles(path);
                    if (files.Any(x => x.Contains(file.FileInfo.Name)))
                    {
                        var fileNameSplitted = file.FileInfo.Name.Split($".{file.FileInfo.Type}");

                        file.FileInfo.Name = fileNameSplitted.FirstOrDefault() + DateTime.Now.ToString("ddMMyyyyHHmmss") + "." + file.FileInfo.Type;
                    }
                }
            }
        }

        private async Task SetFileNameAsync()
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + this.candidateProviderTrainerDocumentVM.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                this.candidateProviderTrainerDocumentVM.FileName = string.Join(Environment.NewLine, files);
            }
        }

        private async Task FilesSelectedHandler(SelectedEventArgs args)
        {
            var maxFilesCount = int.Parse((await this.DataSourceService.GetSettingByIntCodeAsync("MaxFilesCount")).SettingValue);
            var uploadedFilesCount = await this.GetFilesCountAsync();
            var selectedPaths = await this.uploader.GetFilesDataAsync();
            if (uploadedFilesCount + selectedPaths.Count + args.FilesData.Count > maxFilesCount)
            {
                args.Cancel = true;
                string count = maxFilesCount == 1 ? "файл" : "файла";
                await this.ShowErrorAsync($"Не можете да прикачите повече от {maxFilesCount} {count}!");
            }
        }

        private async Task<int> GetFilesCountAsync()
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var filePathMain = $"\\UploadedFiles\\CandidateProviderTrainerDocument\\{this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument}";
            var filePath = settingResource + filePathMain;
            if (Directory.Exists(filePath))
            {
                return Directory.GetFiles(filePath).Length;
            }

            return 0;
        }

        private async Task SubmitAndContinueBtn()
        {
            await this.SubmitDocumentHandler();

            if (!this.editContext.GetValidationMessages().Any())
            {
                var idCandidateProviderTrainer = this.candidateProviderTrainerDocumentVM.IdCandidateProviderTrainer;

                this.candidateProviderTrainerDocumentVM = new CandidateProviderTrainerDocumentVM()
                {
                    IdCandidateProviderTrainer = idCandidateProviderTrainer
                };
            }
        }
    }
}
