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
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOCandidateProviderPremisesDocumentModal : BlazorBaseComponent
    {
        SfDialog sfDialog = new SfDialog();
        private SfUploader uploader = new SfUploader();

        private CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM = new CandidateProviderPremisesDocumentVM();
        private IEnumerable<KeyValueVM> kvDocumentTypeSource = new List<KeyValueVM>();

        [Parameter]
        public EventCallback<CandidateProviderPremisesDocumentVM> CallbackAfterModalSubmit { get; set; }

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
            this.editContext = new EditContext(this.candidateProviderPremisesDocumentVM);
        }

        private async Task SubmitDocumentHandler()
        {
            ResultContext<CandidateProviderPremisesDocumentVM> resultContext = new ResultContext<CandidateProviderPremisesDocumentVM>();

            this.editContext = new EditContext(this.candidateProviderPremisesDocumentVM);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {

                if (this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument != 0)
                {
                    resultContext = await this.CandidateProviderService.UpdateCandidateProviderPremisesDocumentAsync(this.candidateProviderPremisesDocumentVM);
                }
                else
                {
                    resultContext = await this.CandidateProviderService.CreateCandidateProviderPremisesDocumentAsync(this.candidateProviderPremisesDocumentVM);
                }

                if (resultContext.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                }

                await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderPremisesDocumentVM);
            }
        }

        public async Task OpenModal(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM, IEnumerable<KeyValueVM> kvDocumentTypeSource)
        {
            this.kvDocumentTypeSource = kvDocumentTypeSource.Where(k => k.DefaultValue3.Contains("CIPO")).ToList();

            if (candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument != 0)
            {
                this.candidateProviderPremisesDocumentVM = await this.CandidateProviderService.GetCandidateProviderPremisesDocumentByIdAsync(candidateProviderPremisesDocumentVM);
            }
            else
            {
                this.candidateProviderPremisesDocumentVM = new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = candidateProviderPremisesDocumentVM.IdCandidateProviderPremises };
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task OnRemoveClick(string fileName)
        {
            if (this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument > 0)
            {
                string msg = "Сигурни ли си сте, че искате да изтриете прикачения файл?";
                bool isConfirmed = await this.ShowConfirmDialogAsync(msg);                
                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAndFileNameAsync<CandidateProviderPremisesDocument>(this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument, fileName);

                    if (result == 1)
                    {
                        this.candidateProviderPremisesDocumentVM = await this.CandidateProviderService.GetCandidateProviderPremisesDocumentByIdAsync(this.candidateProviderPremisesDocumentVM);
                        await this.SetFileNameAsync();

                        this.StateHasChanged();

                        await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderPremisesDocumentVM);
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

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderPremisesDocument>(this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderPremisesDocument>(this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument, fileName);

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
                if (this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument > 0)
                {
                    bool isConfirmed = true;

                    if (isConfirmed)
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAndFileNameAsync<CandidateProviderPremisesDocument>(this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument, args.FilesData[0].Name);

                        if (result == 1)
                        {
                            this.candidateProviderPremisesDocumentVM = await this.CandidateProviderService.GetCandidateProviderPremisesDocumentByIdAsync(this.candidateProviderPremisesDocumentVM);

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

                var result = await this.UploadFileService.UploadFileCandidateProviderPremisesDocumentAsync(fileStreams, fileName, "CandidateProviderPremisesDocument", this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument);

                this.candidateProviderPremisesDocumentVM.UploadedFileName = result.ResultContextObject.UploadedFileName;

                await this.SetFileNameAsync();

                this.StateHasChanged();

                this.editContext = new EditContext(this.candidateProviderPremisesDocumentVM);
            }
        }

        private async Task SetNameForFilesWithSameFileNameAsync(UploadChangeEventArgs args)
        {
            var settingFolder = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            foreach (var file in args.Files)
            {
                if (!string.IsNullOrEmpty(this.candidateProviderPremisesDocumentVM.UploadedFileName))
                {
                    var path = settingFolder + "\\" + this.candidateProviderPremisesDocumentVM.UploadedFileName;
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
            var fileFullName = settingResource + "\\" + this.candidateProviderPremisesDocumentVM.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                this.candidateProviderPremisesDocumentVM.FileName = string.Join(Environment.NewLine, files);
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
            var filePathMain = $"\\UploadedFiles\\CandidateProviderPremisesDocument\\{this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument}";
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
                var idCandidateProviderPremises = this.candidateProviderPremisesDocumentVM.IdCandidateProviderPremises;

                this.candidateProviderPremisesDocumentVM = new CandidateProviderPremisesDocumentVM()
                {
                    IdCandidateProviderPremises = idCandidateProviderPremises
                };
            }
        }
    }
}
