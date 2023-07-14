using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO.ProcedureModalReports
{
    partial class ExternalExpertsReport : BlazorBaseComponent
    {
        #region Inject
        [Inject]
        public IProfessionalDirectionService professionalDirectionService  { get; set; }

        [Inject]
        public IExpertService expertService { get; set; }

        [Inject]
        public IProviderService providerService { get; set; }
        #endregion

        private ToastMsg toast;
        private SfGrid<ExpertVM> sfGrid = new SfGrid<ExpertVM>();
        private IEnumerable<ProfessionalDirectionVM> professionalDirectionDataSource = new List<ProfessionalDirectionVM>();
        private IEnumerable<ExpertVM> externalExpertDatasource = new List<ExpertVM>();
        private IEnumerable<ExpertVM> ExpertGridSource = new List<ExpertVM>();
        private ExternalExpertsReportModal model = new ExternalExpertsReportModal();
        private IEnumerable<ProcedureExternalExpertVM> procedureReports = new List<ProcedureExternalExpertVM>();
        private bool IsDateValid = true;
        public async Task OpenModal(int? idProfessionalDirection, int idExternalExpert, int IdCandidate_Provider, string type = "CPO")
        {
            if (IdCandidate_Provider == 0)
            {
                this.professionalDirectionDataSource = await this.professionalDirectionService.GetAllActiveProfessionalDirectionsAsync();
            }
            else if (type == "CPO")
            {
                this.professionalDirectionDataSource = await this.professionalDirectionService.GetAllProfessionalDirectionsByCandidateProviderIdAsync(IdCandidate_Provider);
            }
            else
            {
                this.professionalDirectionDataSource = (await this.professionalDirectionService.GetAllActiveProfessionalDirectionsAsync()).Where(d => d.Code == "999").ToList();
            }


            this.professionalDirectionDataSource = this.professionalDirectionDataSource.OrderBy(p => p.Code).ToList();
            model = new ExternalExpertsReportModal();
            if (idProfessionalDirection.HasValue && idExternalExpert != 0)
            {
                this.model.IdProfessionalDirection = idProfessionalDirection.Value;
                this.externalExpertDatasource = await this.expertService.GetAllExpertsAsync(new ExpertVM() { IdProfessionalDirectionFilter = this.model.IdProfessionalDirection });
                this.externalExpertDatasource = this.externalExpertDatasource.Where(e => e.IsExternalExpert).ToList();
                this.model.IdExternalExpert = idExternalExpert;
            }
            else if (idProfessionalDirection.HasValue)
            {
                this.model.IdProfessionalDirection = idProfessionalDirection.Value;
                this.externalExpertDatasource = await this.expertService.GetAllExpertsAsync(new ExpertVM() { IdProfessionalDirectionFilter = this.model.IdProfessionalDirection });
                this.externalExpertDatasource = this.externalExpertDatasource.Where(e => e.IsExternalExpert).ToList();
            }
            this.ExpertGridSource = new List<ExpertVM>();
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Submit()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                if (IsDateValid)
                {
                    this.SpinnerShow();
                    var allExperts = await this.expertService.GetAllExpertsAsync(new ExpertVM());
                    //Проверки за полетата 'Професионално направление' и 'Външен експерт'
                    if (model.IdExternalExpert == 0)
                    {
                        if (this.model.IdProfessionalDirection != 0)
                            allExperts = await this.expertService.GetAllExpertsAsync(new ExpertVM() { IdProfessionalDirectionFilter = this.model.IdProfessionalDirection });


                        allExperts = allExperts.Where(e => e.IsExternalExpert).ToList();
                    }
                    else
                    {
                        allExperts = allExperts.Where(e => e.IsExternalExpert && e.IdExpert == model.IdExternalExpert).ToList();
                    }

                    //Проверка за полетата 'Период от/до' 
                    if (model.DateFrom.HasValue && model.DateTo.HasValue)
                    {
                        allExperts = allExperts.Where(e => e.ProcedureExternalExperts.Any(p => (p.StartedProcedure.ExpertReportDeadline.HasValue && model.DateFrom.HasValue ? p.StartedProcedure.ExpertReportDeadline.Value.Date >= model.DateFrom.Value.Date : false) && (p.StartedProcedure.ExpertReportDeadline.HasValue && model.DateTo.HasValue ? p.StartedProcedure.ExpertReportDeadline.Value.Date <= model.DateTo.Value.Date : false)));
                    }
                    else if (model.DateFrom.HasValue && !model.DateTo.HasValue)
                    {
                        allExperts = allExperts.Where(e => e.ProcedureExternalExperts.Any(p => (p.StartedProcedure.ExpertReportDeadline.HasValue && model.DateFrom.HasValue ? p.StartedProcedure.ExpertReportDeadline.Value.Date >= model.DateFrom.Value.Date : false)));
                    }
                    else if (model.DateTo.HasValue && !model.DateFrom.HasValue)
                    {
                        allExperts = allExperts.Where(e => e.ProcedureExternalExperts.Any(p => (p.StartedProcedure.ExpertReportDeadline.HasValue && model.DateTo.HasValue ? p.StartedProcedure.ExpertReportDeadline.Value.Date <= model.DateTo.Value.Date : false)));
                    }

                    this.SpinnerHide();
                    //Пълнене на данните в грида
                    this.ExpertGridSource = allExperts;

                }
                else
                {
                    toast.sfErrorToast.Content = "Въведената дата в полето 'Период от' не може да е след 'Период до'!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task FilterExternalExpert()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.model.IdExternalExpert = GlobalConstants.INVALID_ID;
                if (this.model.IdProfessionalDirection == 0)
                {
                    this.model.IdExternalExpert = GlobalConstants.INVALID_ID;
                    externalExpertDatasource = new List<ExpertVM>();
                }
                else
                {
                    this.externalExpertDatasource = await this.expertService.GetAllExpertsAsync(new ExpertVM() { IdProfessionalDirectionFilter = this.model.IdProfessionalDirection });
                    this.externalExpertDatasource = this.externalExpertDatasource.Where(e => e.IsExternalExpert).ToList();
                    this.StateHasChanged();
                }
            }
            finally
            {
                this.loading = false;
            }

        }

        private void DateValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.model.DateFrom.HasValue)
            {
                startDate = this.model.DateFrom.Value.Date;
            }
            if (this.model.DateTo.HasValue)
            {
                endDate = this.model.DateTo.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);

            if (result > 0 && this.model.DateTo.HasValue && this.model.DateFrom.HasValue)
            {
                this.IsDateValid = false;
            }
            else
            {
                this.IsDateValid = true;
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = ExpertGridSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                //            List<GridColumn> ExportColumns = new List<GridColumn>();
                //#pragma warning disable BL0005
                //            ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                //            ExportColumns.Add(new GridColumn() { Field = "Person.FirstName", HeaderText = "Име", Width = "180", TextAlign = TextAlign.Left });
                //            ExportColumns.Add(new GridColumn() { Field = "Person.SecondName", HeaderText = "Презиме", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                //            ExportColumns.Add(new GridColumn() { Field = "Person.FamilyName", HeaderText = "Фамилия", Width = "80", TextAlign = TextAlign.Left });
                //            ExportColumns.Add(new GridColumn() { Field = "ProfessionalDirectionsInfo", HeaderText = "Професионално направление", Width = "80", TextAlign = TextAlign.Left });
                //            ExportColumns.Add(new GridColumn() { Field = "ProcedureCount", HeaderText = "Брой процедури", Width = "80", TextAlign = TextAlign.Left });
                //#pragma warning restore BL0005

                //            ExportProperties.Columns = ExportColumns;
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
                ExportProperties.FileName = $"ExternalExperts_Report_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ExternalExperts_Report_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ExpertVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdExpert).Result.ToString();
            }
        }

    }


    class ExternalExpertsReportModal
    {
        public int IdExternalExpert { get; set; }

        public int IdProfessionalDirection { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        //TODO: Изготвени Доклади
    }
}
