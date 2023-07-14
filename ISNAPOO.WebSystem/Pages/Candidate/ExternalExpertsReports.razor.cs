using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ExternalExpertsReports : BlazorBaseComponent
    {
        private SfGrid<ProcedureExternalExpertVM> reportsGrid = new SfGrid<ProcedureExternalExpertVM>();
        private ExternalExpertDocumentModal externalExpertDocumentModal = new ExternalExpertDocumentModal();

        private List<ProcedureExternalExpertVM> reportsSource = new List<ProcedureExternalExpertVM>();
        private int idExpert = 0;
        private bool isInRoleNAPOOExpert = false;
        private bool isInRoleExternalExpert = false;
        private bool hideUploadBtn = false;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IProviderService ProviderService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await this.LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            this.hideUploadBtn = this.GetUserRoles().Any(x => x == "EXPERT_COMMITTEES");
            this.reportsSource = (await this.ProviderService.GetAllProcedureExternalExpertReportsByIdStartedProcedureAsync(this.CandidateProviderVM.IdStartedProcedure.Value)).ToList();
            this.reportsSource = this.reportsSource.OrderBy(x => x.ProfessionalDirection.DisplayNameAndCode).ToList();
            this.isInRoleExternalExpert = await this.IsInRole("EXTERNAL_EXPERTS");
            if (this.isInRoleExternalExpert)
            {
                this.idExpert = await this.GetIdExpertByIdPerson();
                this.reportsSource = this.reportsSource.Where(x => x.IdExpert == idExpert).ToList();
            }

            await this.reportsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task<int> GetIdExpertByIdPerson()
        {
            return await this.ProviderService.GetIdExpertByIdPersonAsync(this.UserProps.IdPerson);
        }

        private void AddNewReportHandler(ProcedureExternalExpertVM model)
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

                this.externalExpertDocumentModal.OpenModal(model, CandidateProviderVM);
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
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

                ProcedureExternalExpertVM document = this.reportsSource.FirstOrDefault(x => x.FileName == fileName);

                if (document != null)
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ProcedureExternalExpert>(document.IdProcedureExternalExpert);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ProcedureExternalExpert>(document.IdProcedureExternalExpert);

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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterDocumentSubmitAsync()
        {
            await this.LoadDataAsync();
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
                ExportProperties.FileName = $"External_Experts_Reports_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.reportsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"External_Experts_Reports_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.reportsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "Expert.Person.FullName", HeaderText = "Външен експерт", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ProfessionalDirection.DisplayNameAndCode", HeaderText = "Професионално направление", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "UploadDate", HeaderText = "Дата на прикачване", Format = "dd.MM.yyyy", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "StartedProcedure.ExpertReportDeadline", HeaderText = "Краен срок за представяне на доклад", Format = "dd.MM.yyyy", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FileName", HeaderText = "Прикачен файл", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
