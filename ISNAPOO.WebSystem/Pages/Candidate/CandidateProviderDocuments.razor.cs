using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO.Implementation.Shapes;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderDocuments : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderDocumentsGridData> documentsGrid = new SfGrid<CandidateProviderDocumentsGridData>();
        private CandidateProviderDocumentModal candidateProviderDocumentModal = new CandidateProviderDocumentModal();

        public List<CandidateProviderDocumentsGridData> documentsSource = new List<CandidateProviderDocumentsGridData>();
        private IEnumerable<KeyValueVM> kvProviderDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvMTBDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainerDocumentTypeSource = new List<KeyValueVM>();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsCPO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenOpenFromProfile { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Parameter]
        public bool DisableFieldsWhenApplicationStatusIsNotDocPreparation { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsNAPOO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenActiveLicenceChange { get; set; }

        [Parameter]
        public bool IsUserProfileAdministrator { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                this.kvProviderDocumentTypeSource = this.IsCPO
                    ? (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderDocumentType", false, true)).Where(x => (!string.IsNullOrEmpty(x.DefaultValue3) && x.DefaultValue3.Contains("CPO")) || x.KeyValueIntCode.Equals("MigratedCandidateProviderDocumentType")).ToList()
                    : (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderDocumentType", false, true)).Where(x => (!string.IsNullOrEmpty(x.DefaultValue3) && x.DefaultValue3.Contains("CIPO")) || x.KeyValueIntCode.Equals("MigratedCandidateProviderDocumentType")).ToList();
                
                this.kvMTBDocumentTypeSource = this.IsCPO
                    ? (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType", false, true)).Where(x => !string.IsNullOrEmpty(x.DefaultValue3) && x.DefaultValue3.Contains("CPO")).ToList()
                    : (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType", false, true)).Where(x => !string.IsNullOrEmpty(x.DefaultValue3) && x.DefaultValue3.Contains("CIPO")).ToList();
                this.kvTrainerDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType", false, true);

                await this.LoadDataAsync();

                this.SpinnerHide();
            }
        }

        public async Task LoadDataAsync()
        {
            this.documentsSource = (await this.CandidateProviderService.SetDataForDocumentsGrid(this.CandidateProviderVM.IdCandidate_Provider, this.kvProviderDocumentTypeSource, this.kvMTBDocumentTypeSource, this.kvTrainerDocumentTypeSource)).ToList();
            //foreach (var doc in this.documentsSource)
            //{
            //    await this.SetFileNameAsync(doc);
            //}

            await this.documentsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task AddNewDocumentClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.candidateProviderDocumentModal.OpenModal(new CandidateProviderDocumentVM() { IdCandidateProvider = this.CandidateProviderVM.IdCandidate_Provider }, this.CandidateProviderVM.IdTypeLicense, this.kvProviderDocumentTypeSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnDocumentModalSubmit(CandidateProviderDocumentVM document)
        {
            CandidateProviderDocumentsGridData data = new CandidateProviderDocumentsGridData()
            {
                EntityId = document.IdCandidateProviderDocument,
                EntityType = "CandidateProviderDocument",
                IdDocumentType = document.IdDocumentType,
                DocumentTypeName = document.IdDocumentType != 0 ? kvProviderDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType).Name : string.Empty,
                DocumentTitle = document.DocumentTitle,
                UploadedFileName = document.UploadedFileName,
                UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser),
                CreationDate  = $"{document.CreationDate.ToString("dd.MM.yyyy")} г."
            };

            await this.SetFileNameAsync(data);

            this.documentsSource.RemoveAll(x => x.EntityId == data.EntityId);
            this.documentsSource.Add(data);
            this.documentsSource = this.documentsSource.OrderByDescending(x => DateTime.Parse(x.CreationDate)).ThenBy(x => x.DocumentTypeName).ToList();

            await this.documentsGrid.Refresh();
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
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderDocument>(document.EntityId, fileName.Split("\\").Last());
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderDocument>(document.EntityId, fileName.Split("\\").Last());

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

        private async Task DeleteDocumentBtn(CandidateProviderDocumentsGridData candidateProviderDocumentsGridData)
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
                        await this.HandleResultContextMessages<CandidateProviderDocumentVM>(resultContext, candidateProviderDocumentsGridData.EntityId);
                    }
                    else if (candidateProviderDocumentsGridData.EntityType == "CandidateProviderTrainerDocument")
                    {
                        var resultContext = await this.CandidateProviderService.DeleteCandidateProviderTrainerDocumentAsync(new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainerDocument = candidateProviderDocumentsGridData.EntityId });
                        await this.HandleResultContextMessages<CandidateProviderTrainerDocumentVM>(resultContext, candidateProviderDocumentsGridData.EntityId);
                    }
                    else if (candidateProviderDocumentsGridData.EntityType == "CandidateProviderPremisesDocument")
                    {
                        var resultContext = await this.CandidateProviderService.DeleteCandidateProviderPremisesDocumentAsync(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremisesDocument = candidateProviderDocumentsGridData.EntityId });
                        await this.HandleResultContextMessages<CandidateProviderPremisesDocumentVM>(resultContext, candidateProviderDocumentsGridData.EntityId);
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task HandleResultContextMessages<T>(ResultContext<T> resultContext, int idEntity)
        {
            if (resultContext.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
            }
            else
            {
                await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                this.documentsSource.RemoveAll(x => x.EntityId == idEntity);
                await this.documentsGrid.Refresh();
                this.StateHasChanged();
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                int temp = documentsGrid.PageSettings.PageSize;
                documentsGrid.PageSettings.PageSize = documentsSource.Count();
                await documentsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "DocumentTypeName", HeaderText = "Вид на документа", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentTitle", HeaderText = "Описание на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FileName", HeaderText = "Прикачен файл", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CreationDate", HeaderText = "Дата на прикачване", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "UploadedByName", HeaderText = "Прикачено от", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = $"Prilojeni_Dokumenti_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.documentsGrid.ExportToPdfAsync(ExportProperties);
                documentsGrid.PageSettings.PageSize = temp;
                await documentsGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "DocumentTypeName", HeaderText = "Вид на документа", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentTitle", HeaderText = "Описание на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FileName", HeaderText = "Прикачен файл", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CreationDate", HeaderText = "Дата на прикачване", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "UploadedByName", HeaderText = "Прикачено от", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Prilojeni_Dokumenti_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.documentsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private async Task SetFileNameAsync(CandidateProviderDocumentsGridData candidateProviderTrainerDocument)
        {
            if (!string.IsNullOrEmpty(candidateProviderTrainerDocument.UploadedFileName))
            {
                var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var fileFullName = settingResource + candidateProviderTrainerDocument.UploadedFileName;
                if (Directory.Exists(fileFullName))
                {
                    var files = Directory.GetFiles(fileFullName);
                    files = files.Select(x => x.Split(($"\\{candidateProviderTrainerDocument.EntityId}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                    candidateProviderTrainerDocument.FileName = string.Join(Environment.NewLine, files);
                }
            }
        }
    }
}
