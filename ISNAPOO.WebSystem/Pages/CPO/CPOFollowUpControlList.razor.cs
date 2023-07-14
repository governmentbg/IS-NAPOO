using Data.Models.Data.Control;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.DocIO.DLS;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.CPO
{
    partial class CPOFollowUpControlList : BlazorBaseComponent
    {
        #region Inject
        [Inject]
        public IControlService ControlService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }
        #endregion


        private FollowUpControlModal followUpControlModal;
        private FollowUpControlUpdateDocument followUpControlUpdateDocument = new FollowUpControlUpdateDocument();
        private SfGrid<FollowUpControlVM> sfGrid;
        private IEnumerable<FollowUpControlVM> controlsSource = new List<FollowUpControlVM>();
        private KeyValueVM kvDocumentType = new KeyValueVM();
        private string CPOorCIPO = string.Empty;
        private string LicensingType = string.Empty;


        protected override async Task OnInitializedAsync()
        {
            this.LicensingType = string.Empty;
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = Token;
            tokenContext = BaseHelper.GetDecodeToken(tokenContext);


            this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "FollowUpControlType").Value.ToString();
            if (this.LicensingType == "LicensingCPO")
            {
                CPOorCIPO = "ЦПО";
                this.kvDocumentType = await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CPOBlankReportPK");
            }
            else
            {
                CPOorCIPO = "ЦИПО";
                this.kvDocumentType = await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPOBlankReportPK");
            }
            this.controlsSource = await this.controlService.GetAllControlsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.LicensingType);
            foreach (var control in this.controlsSource)
            {
                foreach (var document in control.FollowUpControlDocuments)
                {
                    if (document.HasUploadedFile)
                    {
                        await this.SetFileNameAsync(document);
                    }
                }
            }
            await this.sfGrid.Refresh();
            this.StateHasChanged();
        }
        public void OpenNewModal()
        {
            this.followUpControlModal.OpenModal(new FollowUpControlVM());
        }

        private async Task SelectedRow(FollowUpControlVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewFollowUpControlData", false);
            if (!hasPermission) { return; }

            var documents = await this.ControlService.GetAllDocumentsAsync(_model.IdFollowUpControl);

            if (documents.Any(d => d.IdDocumentType == kvDocumentType.IdKeyValue))
            {
                this.followUpControlUpdateDocument.OpenModal(documents.FirstOrDefault(d => d.IdDocumentType == kvDocumentType.IdKeyValue), this.CPOorCIPO);
            }
            else
            {
                this.followUpControlUpdateDocument.OpenModal(new FollowUpControlDocumentVM() { IdFollowUpControl = _model.IdFollowUpControl }, this.CPOorCIPO);
            }
        }

        private async Task UpdateAfterSave()
        {
            this.controlsSource = await this.controlService.GetAllControlsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.LicensingType);
            foreach (var control in this.controlsSource)
            {
                foreach (var document in control.FollowUpControlDocuments)
                {
                    if (!string.IsNullOrEmpty(document.UploadedFileName))
                    {
                        await this.SetFileNameAsync(document);
                    }
                    else
                    {
                        document.FileName = null;
                    }
                }
            }

            await this.sfGrid.Refresh();
            this.StateHasChanged();
        }

        public async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = controlsSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                PdfTheme Theme = new PdfTheme();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                if (CPOorCIPO == "ЦПО")
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                else
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "80", TextAlign = TextAlign.Left });

                ExportColumns.Add(new GridColumn() { Field = "FollowUpControlTypeName", HeaderText = "Вид на последващия контрол", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlTypeName", HeaderText = "Вид на проверката", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlStartDate", HeaderText = "Срок за проверката от", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "ControlEndDate", HeaderText = "Срок за проверката до", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "TermImplRecommendation", HeaderText = "Срок за изпълнение на препоръки", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left, });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = Syncfusion.Blazor.Grids.PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String },/*you fonts famiy in form of base64string*/
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String },/*you fonts famiy in form of base64string*/
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = "Posledvasht_kontrol.pdf";
                await this.sfGrid.PdfExport(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                if (CPOorCIPO == "ЦПО")
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                else
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "80", TextAlign = TextAlign.Left });

                ExportColumns.Add(new GridColumn() { Field = "FollowUpControlTypeName", HeaderText = "Вид на последващия контрол", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlTypeName", HeaderText = "Вид на проверката", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlStartDate", HeaderText = "Срок за проверката от", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "ControlEndDate", HeaderText = "Срок за проверката до", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "TermImplRecommendation", HeaderText = "Срок за изпълнение на препоръки", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left, });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = "Posledvasht_kontrol.xlsx";
                await this.sfGrid.ExcelExport(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<FollowUpControlVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdFollowUpControl).Result.ToString();
            }
        }

        private async Task OnDownloadClick(string fileName, FollowUpControlDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<FollowUpControlDocument>(model.IdFollowUpControlDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<FollowUpControlDocument>(model.IdFollowUpControlDocument, fileName);
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

        private async Task SetFileNameAsync(FollowUpControlDocumentVM followUpControlDocument)
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + followUpControlDocument.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                files = files.Select(x => x.Split(($"\\{followUpControlDocument.IdFollowUpControlDocument}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                followUpControlDocument.FileName = files.Count() != 0 && !int.TryParse(files[0], out int test)
                    ? string.Join(Environment.NewLine, files)
                    : string.Empty;
            }
            else
            {
                followUpControlDocument.FileName = string.Empty;
            }
        }


    }
}
