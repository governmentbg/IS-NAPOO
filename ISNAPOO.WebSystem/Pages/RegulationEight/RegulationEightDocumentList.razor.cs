using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.RegulationEight
{
    public partial class RegulationEightDocumentList : BlazorBaseComponent
    {
        private SfGrid<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsGrid = new SfGrid<TypeOfRequestedDocumentVM>();
        private RegulationEightDocumentModal regulationEightDocumentModal = new RegulationEightDocumentModal();

        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private IEnumerable<KeyValueVM> typeFrameworkPrograms = new List<KeyValueVM>();

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await this.LoadDataAsync();
        }

        private void OpenAddNewTypeOfRequestedDocumentModalBtn()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.regulationEightDocumentModal.OpenModal(new TypeOfRequestedDocumentVM(), this.typeFrameworkPrograms);
            }
            finally
            {
                this.loading = false;
            }
        }

        private void OpenEditTypeOfRequestedDocumentModalBtn(TypeOfRequestedDocumentVM typeOfRequestedDocument)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.regulationEightDocumentModal.OpenModal(typeOfRequestedDocument, this.typeFrameworkPrograms);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task UpdateAfterRegulationEightDocumentModalSubmit()
        {
            await this.LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            this.typeOfRequestedDocumentsSource = (await this.ProviderDocumentRequestService.GetAllTypesOfRequestedDocumentAsync()).ToList();
            this.typeFrameworkPrograms = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");

            await this.typeOfRequestedDocumentsGrid.Refresh();
            this.StateHasChanged();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

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
                    ExportProperties.FileName = $"Documents_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                    await this.typeOfRequestedDocumentsGrid.ExportToPdfAsync(ExportProperties);
                }
                else if (args.Item.Id.Contains("excelexport"))
                {
                    ExcelExportProperties ExportProperties = new ExcelExportProperties();
                    ExportProperties.FileName = $"Documents_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                    ExportProperties.IncludeTemplateColumn = true;

                    ExportProperties.Columns = this.SetGridColumnsForExport();
                    await this.typeOfRequestedDocumentsGrid.ExportToExcelAsync(ExportProperties);
                }
            }
            finally
            {
                this.loading = false;
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "DocTypeOfficialNumber", HeaderText = "Номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocTypeName", HeaderText = "Вид на документа", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Price", HeaderText = "Ед. цена", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Order", HeaderText = "Номер по ред", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "HasSerialNumberAsText", HeaderText = "Има фабр. номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "IsDestroyableAsText", HeaderText = "Унищожаване", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CourseTypeName", HeaderText = "Вид на курс", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentStatus", HeaderText = "Статус", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
