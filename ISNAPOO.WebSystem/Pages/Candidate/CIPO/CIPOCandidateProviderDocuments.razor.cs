using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOCandidateProviderDocuments : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderDocumentsGridData> documentsGrid = new SfGrid<CandidateProviderDocumentsGridData>();

        public List<CandidateProviderDocumentsGridData> documentsSource = new List<CandidateProviderDocumentsGridData>();
        private IEnumerable<KeyValueVM> kvProviderDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvMTBDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainerDocumentTypeSource = new List<KeyValueVM>();
        private CIPOCandidateProviderDocumentModal candidateProviderDocumentModal = new CIPOCandidateProviderDocumentModal();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool DisableAllFields { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerShow();

            this.kvProviderDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderDocumentType");
            this.kvMTBDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType");
            this.kvTrainerDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType");
            //this.documentsSource = this.CandidateProviderService.SetDataForDocumentsGrid(this.CandidateProviderVM.IdCandidate_Provider, this.kvProviderDocumentTypeSource, this.kvMTBDocumentTypeSource, this.kvTrainerDocumentTypeSource).ToList();

            this.SpinnerHide();
        }

        private async Task AddNewDocumentClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            await this.candidateProviderDocumentModal.OpenModal(new CandidateProviderDocumentVM() { IdCandidateProvider = this.CandidateProviderVM.IdCandidate_Provider }, this.kvProviderDocumentTypeSource);
        }

        private async Task OnDocumentModalSubmit(CandidateProviderDocumentVM document)
        {
            //this.documentsSource = (await this.CandidateProviderService.SetDataForDocumentsGrid(this.CandidateProviderVM.IdCandidate_Provider, this.kvProviderDocumentTypeSource, this.kvMTBDocumentTypeSource, this.kvTrainerDocumentTypeSource)).ToList();
            CandidateProviderDocumentsGridData data = new CandidateProviderDocumentsGridData()
            {
                EntityId = document.IdCandidateProviderDocument,
                EntityType = "CandidateProviderDocument",
                IdDocumentType = document.IdDocumentType,
                DocumentTypeName = document.IdDocumentType != 0 ? kvProviderDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType).Name : string.Empty,
                DocumentTitle = document.DocumentTitle,
                UploadedFileName = document.UploadedFileName,
                UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser),
                CreationDate = document.CreationDate.ToString("dd.MM.yyyy")
            };

            await this.SetFileNameAsync(data);

            this.documentsSource.RemoveAll(x => x.EntityId == data.EntityId);
            this.documentsSource.Add(data);
            this.documentsSource = this.documentsSource.OrderByDescending(x => DateTime.Parse(x.CreationDate)).ToList();

            this.StateHasChanged();
        }

        private async Task OnDownloadClick(string fileName, CandidateProviderDocumentsGridData entity)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                CandidateProviderDocumentsGridData document = this.documentsSource.FirstOrDefault(x => x.EntityId == entity.EntityId && x.EntityType == entity.EntityType);

                if (document != null)
                {
                    if (document.EntityType == "CandidateProviderDocument")
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderDocument>(document.EntityId, fileName);
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderDocument>(document.EntityId, fileName);

                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.JsRuntime, document.FileName, documentStream.MS!.ToArray());
                            }
                        }
                        else
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                            await this.ShowErrorAsync(msg);
                        }
                    }
                    else if (document.EntityType == "CandidateProviderPremisesDocument")
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderPremisesDocument>(document.EntityId, fileName);
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderPremisesDocument>(document.EntityId, fileName);

                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.JsRuntime, document.FileName, documentStream.MS!.ToArray());
                            }
                        }
                        else
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                            await this.ShowErrorAsync(msg);
                        }
                    }
                    else if (document.EntityType == "CandidateProviderTrainerDocument")
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderTrainerDocument>(document.EntityId, fileName);
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderTrainerDocument>(document.EntityId, fileName);

                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.JsRuntime, document.FileName, documentStream.MS!.ToArray());
                            }
                        }
                        else
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                            await this.ShowErrorAsync(msg);
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteDocument(CandidateProviderDocumentsGridData candidateProviderDocumentsGridData)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    if (candidateProviderDocumentsGridData.EntityType == "CandidateProviderDocument")
                    {
                        var resultContext = await this.CandidateProviderService.DeleteCandidateProviderDocumentAsync(new CandidateProviderDocumentVM() { IdCandidateProviderDocument = candidateProviderDocumentsGridData.EntityId });
                        await this.HandleResultContextMessages<CandidateProviderDocumentVM>(resultContext);
                    }
                    else if (candidateProviderDocumentsGridData.EntityType == "CandidateProviderTrainerDocument")
                    {
                        var resultContext = await this.CandidateProviderService.DeleteCandidateProviderTrainerDocumentAsync(new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainerDocument = candidateProviderDocumentsGridData.EntityId });
                        await this.HandleResultContextMessages<CandidateProviderTrainerDocumentVM>(resultContext);
                    }
                    else if (candidateProviderDocumentsGridData.EntityType == "CandidateProviderPremisesDocument")
                    {
                        var resultContext = await this.CandidateProviderService.DeleteCandidateProviderPremisesDocumentAsync(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremisesDocument = candidateProviderDocumentsGridData.EntityId });
                        await this.HandleResultContextMessages<CandidateProviderPremisesDocumentVM>(resultContext);
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task HandleResultContextMessages<T>(ResultContext<T> resultContext)
        {
            if (resultContext.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
            }
            else
            {
                await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                this.documentsSource = (await this.CandidateProviderService.SetDataForDocumentsGrid(this.CandidateProviderVM.IdCandidate_Provider, this.kvProviderDocumentTypeSource, this.kvMTBDocumentTypeSource, this.kvTrainerDocumentTypeSource)).ToList();
                this.StateHasChanged();
            }
        }

        private async Task SetFileNameAsync(CandidateProviderDocumentsGridData candidateProviderTrainerDocument)
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + candidateProviderTrainerDocument.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                files = files.Select(x => x.Split(($"\\{candidateProviderTrainerDocument.EntityId}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                candidateProviderTrainerDocument.FileName = string.Join(Environment.NewLine, files);
            }
        }
    }
}
