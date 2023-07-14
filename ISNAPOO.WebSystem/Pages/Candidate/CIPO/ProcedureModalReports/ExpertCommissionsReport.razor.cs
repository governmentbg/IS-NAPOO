using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
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
    partial class ExpertCommissionsReport : BlazorBaseComponent
    {
        private ToastMsg toast;
        private SfGrid<ExpertCommissionsReportModal> sfGrid = new SfGrid<ExpertCommissionsReportModal>();
        private IEnumerable<KeyValueVM> expertCommissionsDataSource = new List<KeyValueVM>();
        private List<ExpertCommissionsReportModal> expertCommissionsSourceGrid = new List<ExpertCommissionsReportModal>();
        private ExpertCommissionsReportModal model = new ExpertCommissionsReportModal();
        private bool IsDateValid = true;

        protected override async Task OnInitializedAsync()
        {
            this.expertCommissionsDataSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");
        }

        public async Task OpenModal(int idExpertCommission, string type = "CPO")
        {
            model = new ExpertCommissionsReportModal();
            if (idExpertCommission != 0)
            {
                this.model.IdExpertCommission = idExpertCommission;
            }
            expertCommissionsSourceGrid = new List<ExpertCommissionsReportModal>();
            if (type == "CIPO")
            {
                this.expertCommissionsDataSource = this.expertCommissionsDataSource.Where(c => c.Name.Contains("17. Професионално ориентиране")).ToList();
            }
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Submit()
        {
            if (IsDateValid)
            {
                this.SpinnerShow();
                var allProcedures = await providerService.GetAllProcedureExpertExpertCommissionsAsync();
                List<ExpertCommissionsReportModal> expertCommissions = new List<ExpertCommissionsReportModal>();
                expertCommissionsSourceGrid.Clear();

                if (model.IdExpertCommission == 0)
                {
                    foreach (var item in expertCommissionsDataSource)
                    {
                        expertCommissions.Add(new ExpertCommissionsReportModal()
                        {
                            IdExpertCommission = item.IdKeyValue,
                            ExpertCommissionName = item.Name,
                            ProcedureCount = allProcedures.Where(p => p.IdExpertCommission == item.IdKeyValue).ToList().Count
                        });
                    }
                }
                else
                {
                    foreach (var item in expertCommissionsDataSource)
                    {
                        if (item.IdKeyValue == model.IdExpertCommission)
                        {
                            expertCommissions.Add(new ExpertCommissionsReportModal()
                            {
                                IdExpertCommission = item.IdKeyValue,
                                ExpertCommissionName = item.Name,
                                ProcedureCount = allProcedures.Where(p => p.IdExpertCommission == item.IdKeyValue).ToList().Count
                            });
                        }
                    }
                }

                //Проверка за полетата 'Период от/до' 
                if (model.DateFrom.HasValue && model.DateTo.HasValue)
                {
                    allProcedures = allProcedures.Where(e => (e.StartedProcedure.NapooReportDeadline.HasValue && model.DateFrom.HasValue ? e.StartedProcedure.NapooReportDeadline.Value.Date >= model.DateFrom.Value.Date : false) && (e.StartedProcedure.NapooReportDeadline.HasValue && model.DateTo.HasValue ? e.StartedProcedure.NapooReportDeadline.Value.Date <= model.DateTo.Value.Date : false)).ToList();
                }
                else if (model.DateFrom.HasValue && !model.DateTo.HasValue)
                {
                    allProcedures = allProcedures.Where(e => (e.StartedProcedure.NapooReportDeadline.HasValue && model.DateFrom.HasValue ? e.StartedProcedure.NapooReportDeadline.Value.Date >= model.DateFrom.Value.Date : false));
                }
                else if (model.DateTo.HasValue && !model.DateFrom.HasValue)
                {
                    allProcedures = allProcedures.Where(e => (e.StartedProcedure.NapooReportDeadline.HasValue && model.DateTo.HasValue ? e.StartedProcedure.NapooReportDeadline.Value.Date <= model.DateTo.Value.Date : false));
                }

                if (model.DateFrom.HasValue || model.DateTo.HasValue)
                {
                    List<ExpertCommissionsReportModal> expertCommissionsSorted = new List<ExpertCommissionsReportModal>();

                    foreach (var item in allProcedures)
                    {
                        if (!expertCommissionsSorted.Any(e => e.IdExpertCommission == item.IdExpertCommission))
                        {
                            if (expertCommissions.Any(e => e.IdExpertCommission == item.IdExpertCommission))
                            {
                                expertCommissionsSorted.Add(expertCommissions.First(e => e.IdExpertCommission == item.IdExpertCommission));
                            }
                        }
                    }
                    expertCommissions = expertCommissionsSorted;
                }

                this.expertCommissionsSourceGrid = expertCommissions;
                await sfGrid.Refresh();
                this.SpinnerHide();

            }
            else
            {
                toast.sfErrorToast.Content = "Въведената дата в полето 'Период от' не може да е след 'Период до'!";
                await toast.sfErrorToast.ShowAsync();
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
                sfGrid.PageSettings.PageSize = expertCommissionsSourceGrid.Count();
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
                ExportProperties.FileName = $"ExpertCommissions_Report_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ExpertCommissions_Report_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        //protected void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ExpertCommissionsReportModal> args)
        //{
        //    if (args.Column.HeaderText == " ")
        //    {
        //        args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdExpertCommission).Result.ToString();
        //    }
        //}
    }

    class ExpertCommissionsReportModal
    {
        public int IdExpertCommission { get; set; }

        public string ExpertCommissionName { get; set; }

        public int ProcedureCount { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        //TODO: Изготвени Доклади
    }
}
