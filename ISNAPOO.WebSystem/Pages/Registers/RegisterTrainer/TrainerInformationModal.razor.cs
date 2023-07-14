using Data.Models.Data.Candidate;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterTrainer
{
    public partial class TrainerInformationModal : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderTrainerProfileVM> profilesGrid = new SfGrid<CandidateProviderTrainerProfileVM>();
        private SfGrid<CandidateProviderTrainerQualificationVM> qualificationsGrid = new SfGrid<CandidateProviderTrainerQualificationVM>();
        private SfGrid<CandidateProviderTrainerDocumentVM> documentsGrid = new SfGrid<CandidateProviderTrainerDocumentVM>();
        private CandidateProviderTrainerVM candidateProviderTrainerVM = new CandidateProviderTrainerVM();
        private List<CandidateProviderTrainerProfileVM> profilesSource = new List<CandidateProviderTrainerProfileVM>();
        private List<CandidateProviderTrainerQualificationVM> qualificationsSource = new List<CandidateProviderTrainerQualificationVM>();
        private List<CandidateProviderTrainerDocumentVM> documentsSource = new List<CandidateProviderTrainerDocumentVM>();
        private bool isCPO = true;
        private string title = string.Empty;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IProfessionalDirectionService ProfessionalDirectionService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.candidateProviderTrainerVM);
        }

        public async Task OpenModal(int idCandidateProviderTrainer, bool isCPO)
        {
            this.editContext = new EditContext(this.candidateProviderTrainerVM);

            this.isCPO = isCPO;

            await this.LoadDataAsync(idCandidateProviderTrainer);

            this.SetTitle();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadDataAsync(int idCandidateProviderTrainer)
        {           
            this.candidateProviderTrainerVM = await this.CandidateProviderService.GetCandidateProviderTrainerForRegisterByIdAsync(idCandidateProviderTrainer);
            var kvActiveOrInActive = await this.DataSourceService.GetKeyValueByIdAsync(this.candidateProviderTrainerVM.IdStatus);
            
            if (kvActiveOrInActive != null)
            {
                this.candidateProviderTrainerVM.StatusName = kvActiveOrInActive.Name;
            }
            
            if (this.isCPO)
            {
                this.qualificationsSource = this.candidateProviderTrainerVM.CandidateProviderTrainerQualifications.ToList();

                this.profilesSource = this.candidateProviderTrainerVM.CandidateProviderTrainerProfiles.ToList();
            }

            this.documentsSource = this.candidateProviderTrainerVM.CandidateProviderTrainerDocuments.ToList();
            foreach (var doc in this.documentsSource)
            {
                await this.SetFileNameAsync(doc);
            }
        }

        private void SetTitle()
        {
            this.title = this.isCPO
                ? $"Данни за преподавател <span style=\"color: #ffffff;\">{this.candidateProviderTrainerVM.FullName}</span>"
                : $"Данни за консултант <span style=\"color: #ffffff;\">{this.candidateProviderTrainerVM.FullName}</span>";
        }

        private async Task OnDownloadClick(string fileName, CandidateProviderTrainerDocumentVM candidateProviderTrainerDocument)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderTrainerDocument>(candidateProviderTrainerDocument.IdCandidateProviderTrainerDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderTrainerDocument>(candidateProviderTrainerDocument.IdCandidateProviderTrainerDocument, fileName);

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
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SetFileNameAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocument)
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + candidateProviderTrainerDocument.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                files = files.Select(x => x.Split(($"\\{candidateProviderTrainerDocument.IdCandidateProviderTrainerDocument}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                candidateProviderTrainerDocument.FileName = string.Join(Environment.NewLine, files);
            }
            else
            {
                candidateProviderTrainerDocument.FileName = string.Empty;
            }
        }

    }
}
