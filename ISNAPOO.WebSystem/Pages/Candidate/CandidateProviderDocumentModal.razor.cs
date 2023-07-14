using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderDocumentModal : BlazorBaseComponent
    {
        private SfUploader uploader = new SfUploader();
        private KeyValueVM licenceCPO, licenceCIPOO;
        private CandidateProviderDocumentVM candidateProviderDocumentVM = new CandidateProviderDocumentVM();
        private IEnumerable<KeyValueVM> kvDocumentTypeSource = new List<KeyValueVM>();
        private bool IsAdditionalDocument = false;
        private string fileNameForDeletion;
        private bool showDeleteConfirmDialog;

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<CandidateProviderDocumentVM> CallbackAfterModalSubmit { get; set; }

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
            this.editContext = new EditContext(this.candidateProviderDocumentVM);
        }

        private async Task SubmitDocumentHandler()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.candidateProviderDocumentVM);
                this.editContext.EnableDataAnnotationsValidation();

                ResultContext<CandidateProviderDocumentVM> resultContext = new ResultContext<CandidateProviderDocumentVM>();

                if (this.editContext.Validate())
                {
                    if (this.candidateProviderDocumentVM.IdCandidateProviderDocument != 0)
                    {
                        resultContext = await this.CandidateProviderService.UpdateCandidateProviderDocumentAsync(this.candidateProviderDocumentVM, this.IsAdditionalDocument);
                    }
                    else
                    {
                        resultContext = await this.CandidateProviderService.CreateCandidateProviderDocumentAsync(this.candidateProviderDocumentVM, this.IsAdditionalDocument);
                    }

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderDocumentVM);
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task OpenModal(CandidateProviderDocumentVM candidateProviderDocumentVM, int IdTypeLicense, IEnumerable<KeyValueVM> kvDocumentTypeSource, bool isAdditionalDocument = false)
        {
            this.editContext = new EditContext(this.candidateProviderDocumentVM);

            this.kvDocumentTypeSource = Enumerable.Empty<KeyValueVM>().AsQueryable();
            this.IsAdditionalDocument = isAdditionalDocument;
            this.candidateProviderDocumentVM = candidateProviderDocumentVM;
            this.licenceCPO = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO"));
            this.licenceCIPOO = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO"));

            this.candidateProviderDocumentVM = candidateProviderDocumentVM;

            if (this.licenceCPO.IdKeyValue == IdTypeLicense)
            {
                this.kvDocumentTypeSource = kvDocumentTypeSource.Where(k => k.DefaultValue3 != null && k.DefaultValue3.Contains("CPO")).ToList();
            }
            else if (this.licenceCIPOO.IdKeyValue == IdTypeLicense)
            {
                this.kvDocumentTypeSource = kvDocumentTypeSource.Where(k => k.DefaultValue3 != null && k.DefaultValue3.Contains("CIPO")).ToList();
            }

            this.isVisible = true;
            this.StateHasChanged();
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

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderDocument>(this.candidateProviderDocumentVM.IdCandidateProviderDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderDocument>(this.candidateProviderDocumentVM.IdCandidateProviderDocument, fileName);

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

        private async Task OnRemove(string fileName)
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете прикачения файл?");
            if (confirmed)
            {
                if (this.candidateProviderDocumentVM.IdCandidateProviderDocument > 0)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAndFileNameAsync<CandidateProviderDocument>(this.candidateProviderDocumentVM.IdCandidateProviderDocument, fileName);
                    if (result == 1)
                    {
                        this.candidateProviderDocumentVM.UploadedFileName = null;
                        await this.uploader.ClearAllAsync();

                        this.StateHasChanged();
                    }

                    await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderDocumentVM);
                }
            }
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

                if (args.Files.Count >= 1)
                {
                    await this.SetNameForFilesWithSameFileNameAsync(args);

                    var fileNames = args.Files.Select(x => $"{x.FileInfo.Name} ");
                    var fileName = string.Join(Environment.NewLine, fileNames).Trim();
                    var fileStreams = args.Files.Select(x => x.Stream).ToArray();

                    var result = await this.UploadFileService.UploadFileCandidateProviderDocumentAsync(fileStreams, fileName, "CandidateProviderDocument", this.candidateProviderDocumentVM.IdCandidateProviderDocument);

                    this.candidateProviderDocumentVM.UploadedFileName = result.ResultContextObject.UploadedFileName;

                    await this.SetFileNameAsync();

                    this.StateHasChanged();

                    this.editContext = new EditContext(this.candidateProviderDocumentVM);

                    await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderDocumentVM);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SetNameForFilesWithSameFileNameAsync(UploadChangeEventArgs args)
        {
            var settingFolder = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            foreach (var file in args.Files)
            {
                if (!string.IsNullOrEmpty(this.candidateProviderDocumentVM.UploadedFileName))
                {
                    var path = settingFolder + "\\" + this.candidateProviderDocumentVM.UploadedFileName;
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
            var fileFullName = settingResource + "\\" + this.candidateProviderDocumentVM.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                this.candidateProviderDocumentVM.FileName = string.Join(Environment.NewLine, files);
            }
        }

        private async Task FilesSelectedHandler(SelectedEventArgs args)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task<int> GetFilesCountAsync()
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var filePathMain = $"\\UploadedFiles\\CandidateProviderDocument\\{this.candidateProviderDocumentVM.IdCandidateProviderDocument}";
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
                var idCandidateProvider = this.candidateProviderDocumentVM.IdCandidateProvider;

                this.candidateProviderDocumentVM = new CandidateProviderDocumentVM()
                {
                    IdCandidateProvider = idCandidateProvider
                };
            }
        }
    }
}
