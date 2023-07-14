using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class NAPOOTypeOfRequestedDocumentReferenceModal : BlazorBaseComponent
    {
        private SfDialog napooTypeOfRequestedDocumentReferenceModal = new SfDialog();
        private SfGrid<DocumentSerialNumberVM> documentSerialNumbersGrid = new SfGrid<DocumentSerialNumberVM>();

        private RequestDocumentManagementControlModel requestDocumentManagementVM = new RequestDocumentManagementControlModel();
        private List<DocumentSerialNumberVM> documentSerialNumbersSource = new List<DocumentSerialNumberVM>();
        private IEnumerable<DocumentSeriesVM> documentSeriesSource = new List<DocumentSeriesVM>();
        private IEnumerable<KeyValueVM> kvDocumentOperationsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvDocumentReceiveTypeSource = new List<KeyValueVM>();
        private int idReceiveFromMONPrinting = 0;
        private int idReceiveFromOtherCPO = 0;
        private int idDocumentOperation = 0;
        private int idPrinted = 0;

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        public async Task OpenModal(RequestDocumentManagementControlModel requestDocumentManagementVM)
        {
            this.requestDocumentManagementVM = requestDocumentManagementVM;
            this.documentSerialNumbersSource = this.requestDocumentManagementVM.DocumentSerialNumbers.OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();
            this.documentSeriesSource = await this.ProviderDocumentRequestService.GetAllDocumentSeriesAsync();
            this.kvDocumentOperationsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
            this.idDocumentOperation = this.kvDocumentOperationsSource.FirstOrDefault(x => x.KeyValueIntCode == "Submitted").IdKeyValue;
            this.idPrinted = this.kvDocumentOperationsSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed").IdKeyValue;
            this.kvDocumentReceiveTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("DocumentRequestReceiveType");
            this.idReceiveFromMONPrinting = this.kvDocumentReceiveTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "PrintingHouse").IdKeyValue;
            this.idReceiveFromOtherCPO = this.kvDocumentReceiveTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "OtherCPO").IdKeyValue;

            await this.SetDocumentSerialNumberDataForGrid();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SetDocumentSerialNumberDataForGrid()
        {
            foreach (var docSerialNumber in this.documentSerialNumbersSource)
            {
                docSerialNumber.TypeOfRequestedDocument = this.requestDocumentManagementVM.TypeOfRequestedDocument;

                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                if (docSeries is not null)
                {
                    docSerialNumber.DocumentSeriesName = docSeries.SeriesName;
                }

                var docOperationStatus = this.kvDocumentOperationsSource.FirstOrDefault(x => x.IdKeyValue == docSerialNumber.IdDocumentOperation);
                if (docOperationStatus is not null)
                {
                    docSerialNumber.DocumentOperationName = docOperationStatus.Name;
                }

                if (docSerialNumber.RequestDocumentManagement.IdDocumentRequestReceiveType.HasValue)
                {
                    if (docSerialNumber.RequestDocumentManagement.IdDocumentRequestReceiveType == this.idReceiveFromMONPrinting)
                    {
                        docSerialNumber.DocumentReceivedFrom = "печатница";
                    }
                    else if (docSerialNumber.RequestDocumentManagement.IdDocumentRequestReceiveType == this.idReceiveFromOtherCPO)
                    {
                        docSerialNumber.DocumentReceivedFrom = docSerialNumber.RequestDocumentManagement.CandidateProviderPartner.ProviderOwner;
                    }
                }

                if (docSerialNumber.IdDocumentOperation == this.idDocumentOperation)
                {
                    if (docSerialNumber.RequestDocumentManagement.CandidateProviderPartner is not null)
                    {
                        docSerialNumber.DocumentReceivedFrom = docSerialNumber.RequestDocumentManagement.CandidateProviderPartner.ProviderOwner;
                    }
                }
                else if (docSerialNumber.IdDocumentOperation == this.idPrinted)
                {
                    docSerialNumber.DocumentReceivedFrom = await this.ProviderDocumentRequestService.GetClientNameByIdDocumentSerialNumberAsync(docSerialNumber.IdDocumentSerialNumber);
                }
            }
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
                ExportProperties.FileName = $"DocumentSerialNumbersList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.documentSerialNumbersGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"DocumentSerialNumbersList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.documentSerialNumbersGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документ", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentSeriesName", HeaderText = "Серия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SerialNumber", HeaderText = "Фабричен номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentDateAsStr", HeaderText = "Дата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentOperationName", HeaderText = "Статус", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentReceivedFrom", HeaderText = "от/на", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
