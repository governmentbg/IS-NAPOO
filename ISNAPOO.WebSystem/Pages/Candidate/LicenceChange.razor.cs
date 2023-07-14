using DocuServiceReference;
using DocuWorkService;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class LicenceChange : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderLicenceChangeVM> changesGrid = new SfGrid<CandidateProviderLicenceChangeVM>();

        private IEnumerable<CandidateProviderLicenceChangeVM> changesSource = new List<CandidateProviderLicenceChangeVM>();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDocuService docuService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.changesSource = await this.CandidateProviderService.GetCandidateProviderLicensesListByIdAsync(this.CandidateProviderVM.IdCandidate_Provider);

                await this.changesGrid.Refresh();
                this.StateHasChanged();
            }
        }

        public async Task GetLicenceDocumentBtn(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                string guid = string.Empty;
                FileData[] files;
                if (candidateProviderLicenceChangeVM.DS_OFFICIAL_ID != null)
                {
                    var contextResponse = await this.docuService.GetDocumentAsync((int)candidateProviderLicenceChangeVM.DS_OFFICIAL_ID, candidateProviderLicenceChangeVM.DS_OFFICIAL_GUID);

                    if (contextResponse.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                        return;
                    }

                    var doc = contextResponse.ResultContextObject;

                    files = doc.Doc.File;
                    guid = doc.Doc.GUID;

                    if (files == null || files.Count() == 0)
                    {
                        await this.ShowErrorAsync("За въведеният номер на заповед няма прикачен файл в деловодната система!");
                    }
                    else
                    {
                        foreach (var file in files)
                        {
                            var fileResponse = await docuService.GetFileAsync(file.FileID, guid);

                            await FileUtils.SaveAs(JsRuntime, file.Filename, fileResponse.File.BinaryContent.ToArray());
                        }
                    }
                }
                else
                {
                    await this.ShowErrorAsync("За въведеният номер на заповед няма прикачен файл в деловодната система!");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();

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
                ExportProperties.FileName = $"TrainingCourseClientsList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.changesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"TrainingCourseClientsList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.changesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "LicenceStatusName", HeaderText = "Статус на лицензията", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "NumberCommand", HeaderText = "Заповед", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ChangeDateAsStr", HeaderText = "Дата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "LicenceStatusDetailName", HeaderText = "Вид на промяната", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Notes", HeaderText = "Бележки", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Archive", HeaderText = "Съхранение на архива", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
