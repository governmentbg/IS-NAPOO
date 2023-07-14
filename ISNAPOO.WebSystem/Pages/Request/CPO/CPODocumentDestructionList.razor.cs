using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPODocumentDestructionList : BlazorBaseComponent
    {
        private SfGrid<RequestReportVM> requestReportsGrid = new SfGrid<RequestReportVM>();
        private ToastMsg toast = new ToastMsg();
        private CPODocumentDestructionModal cpoDocumentDestructionModal = new CPODocumentDestructionModal();

        private List<RequestReportVM> requestReportsSource = new List<RequestReportVM>();
        private IEnumerable<KeyValueVM> kvRequestReportStatusSource = new List<KeyValueVM>();
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        private bool isInRoleNAPOOExpert = false;
        private int idCreatedKv = 0;
        private RequestReportVM reportToDelete = new RequestReportVM();

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.candidateProviderVM.IdCandidate_Provider = this.UserProps.IdCandidateProvider;
            this.kvRequestReportStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestReportStatus");
            this.idCreatedKv = this.kvRequestReportStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Created").IdKeyValue;

            await this.SetDataForGridAsync();

            await this.requestReportsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task SetDataForGridAsync()
        {
            this.isInRoleNAPOOExpert = await this.IsInRole("NAPOO_Expert");

            if (!this.isInRoleNAPOOExpert)
            {
                this.requestReportsSource = (await this.ProviderDocumentRequestService.GetAllRequestReportsByCandidateProviderIdAsync(this.candidateProviderVM)).ToList();
            }
            else
            {
                this.requestReportsSource = (await this.ProviderDocumentRequestService.GetAllRequestReportsAsync()).ToList();
            }

            foreach (var requestReport in this.requestReportsSource)
            {
                var status = this.kvRequestReportStatusSource.FirstOrDefault(x => x.IdKeyValue == requestReport.IdStatus);
                if (status is not null)
                {
                    requestReport.StatusName = status.Name;
                }
            }
        }

        private async Task AddNewDocumentDestructionBtn()
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.cpoDocumentDestructionModal.OpenModal(new RequestReportVM() { IdCandidateProvider = this.candidateProviderVM.IdCandidate_Provider, IdStatus = this.kvRequestReportStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Created").IdKeyValue }, this.kvRequestReportStatusSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditDocumentDestructionBtn(RequestReportVM requestReportVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var requestReport = await this.ProviderDocumentRequestService.GetRequestReportByIdAsync(requestReportVM);
                await this.cpoDocumentDestructionModal.OpenModal(requestReport, this.kvRequestReportStatusSource, this.isInRoleNAPOOExpert);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterModalSubmit()
        {
            await this.SetDataForGridAsync();
            this.requestReportsGrid.Refresh();
        }

        private async Task DeleteRequestReport(RequestReportVM reqReport)
        {
            this.reportToDelete = reqReport;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                var result = await this.ProviderDocumentRequestService.DeleteRequestReportByIdAsync(reqReport.IdRequestReport);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    await this.SetDataForGridAsync();
                }
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = requestReportsGrid.PageSettings.PageSize;
                requestReportsGrid.PageSettings.PageSize = requestReportsSource.Count();
                await requestReportsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "180", TextAlign = TextAlign.Left });              
                ExportColumns.Add(new GridColumn() { Field = "Year", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DestructionDate", HeaderText = "Дата на отчета", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"CPO_DocumentDestruction_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.requestReportsGrid.ExportToPdfAsync(ExportProperties);
                requestReportsGrid.PageSettings.PageSize = temp;
                await requestReportsGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "250", TextAlign = TextAlign.Left });                
                ExportColumns.Add(new GridColumn() { Field = "Year", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DestructionDate", HeaderText = "Дата на отчета", Width = "100", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус", Width = "70", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.FileName = $"CPO_DocumentDestruction_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.requestReportsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<RequestReportVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(requestReportsGrid, args.Data.IdRequestReport).Result.ToString();
            }
        }
    }
}
