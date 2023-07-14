using System.Data;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class PrintingHouseReportList : BlazorBaseComponent
    {
        private SfGrid<PrintingHouseReportVM> documentRequestsGrid = new SfGrid<PrintingHouseReportVM>();
        private NAPOOSummarizeRequestsModal summarizeRequestsModal = new NAPOOSummarizeRequestsModal();

        private List<PrintingHouseReportVM> documentRequestsSource = new List<PrintingHouseReportVM>();

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.documentRequestsSource = (await this.ProviderDocumentRequestService.GetPrintingHouseReportDataAsync()).ToList();

                this.documentRequestsSource = this.FilterReportsAccordingToUserRoles().ToList();

                await this.documentRequestsGrid.Refresh();
                this.StateHasChanged();
            }
        }

        private List<PrintingHouseReportVM> FilterReportsAccordingToUserRoles()
        {
            var data = new List<PrintingHouseReportVM>();

            var userRoles = this.GetUserRoles();
            if (userRoles.Any(x => x == "PrintingHouse_ShowAll"))
            {
                data.AddRange(this.documentRequestsSource.ToList());
                return data;
            }
            else
            {
                foreach (var role in userRoles)
                {
                    if (role == "PrintingHouse_Blagoevgrad")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Благоевград").ToList());
                    }

                    if (role == "PrintingHouse_Burgas")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Бургас").ToList());
                    }

                    if (role == "PrintingHouse_VelikoTarnovo")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Велико Търново").ToList());
                    }

                    if (role == "PrintingHouse_Vidin")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Видин").ToList());
                    }

                    if (role == "PrintingHouse_Vratsa")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Враца").ToList());
                    }

                    if (role == "PrintingHouse_Gabrovo")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Габрово").ToList());
                    }

                    if (role == "PrintingHouse_Dobritch")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Добрич").ToList());
                    }

                    if (role == "PrintingHouse_Kardzhali")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Кърджали").ToList());
                    }

                    if (role == "PrintingHouse_Kyustendil")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Кюстендил").ToList());
                    }

                    if (role == "PrintingHouse_Lovetch")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Ловеч").ToList());
                    }

                    if (role == "PrintingHouse_Montana")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Монтана").ToList());
                    }

                    if (role == "PrintingHouse_Pazardzhik")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Пазарджик").ToList());
                    }

                    if (role == "PrintingHouse_Pernik")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Перник").ToList());
                    }

                    if (role == "PrintingHouse_Pleven")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Плевен").ToList());
                    }

                    if (role == "PrintingHouse_Plovdiv")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Пловдив").ToList());
                    }

                    if (role == "PrintingHouse_Razgrad")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Разград").ToList());
                    }

                    if (role == "PrintingHouse_Ruse")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Русе").ToList());
                    }

                    if (role == "PrintingHouse_Silistra")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Силистра").ToList());
                    }

                    if (role == "PrintingHouse_Sliven")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Сливен").ToList());
                    }

                    if (role == "PrintingHouse_Smolyan")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Смолян").ToList());
                    }

                    if (role == "PrintingHouse_Sofia")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "София").ToList());
                    }

                    if (role == "PrintingHouse_SofiaCity")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "София (столица)").ToList());
                    }

                    if (role == "PrintingHouse_StaraZagora")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Стара Загора").ToList());
                    }

                    if (role == "PrintingHouse_Targovishte")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Търговище").ToList());
                    }

                    if (role == "PrintingHouse_Haskovo")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Хасково").ToList());
                    }

                    if (role == "PrintingHouse_Shumen")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Шумен").ToList());
                    }

                    if (role == "PrintingHouse_Yambol")
                    {
                        data.AddRange(this.documentRequestsSource.Where(x => x.District == "Ямбол").ToList());
                    }
                }
            }

            return data;
        }

        private async Task OpenRequestDetailsModal(PrintingHouseReportVM printingHouseReport)
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

                var kvRequestDocumetStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestDocumetStatus");
                var entryFromPrintingHouseList = true;
                var napooRequestDocFromDb = await this.ProviderDocumentRequestService.GetNAPOORequestDocDataByIdNAPOORequestDocAsync(printingHouseReport.NAPOORequestDoc.IdNAPOORequestDoc);
                await this.summarizeRequestsModal.OpenModal(napooRequestDocFromDb.ProviderRequestDocuments.ToList(), napooRequestDocFromDb.ProviderRequestDocuments.ToList(), printingHouseReport.NAPOORequestDoc, kvRequestDocumetStatusSource, entryFromPrintingHouseList);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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
                    ExportProperties.FileName = $"Document_requests_report_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                    await this.documentRequestsGrid.ExportToPdfAsync(ExportProperties);
                }
                else if (args.Item.Id.Contains("excelexport"))
                {
                    ExcelExportProperties ExportProperties = new ExcelExportProperties();
                    ExportProperties.FileName = $"Document_requests_report_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                    ExportProperties.IncludeTemplateColumn = true;

                    ExportProperties.Columns = this.SetGridColumnsForExport();
                    await this.documentRequestsGrid.ExportToExcelAsync(ExportProperties);
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

            ExportColumns.Add(new GridColumn() { Field = "District", HeaderText = "Област", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "NumberAndDate", HeaderText = "№ на обобщена заявка/дата", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
