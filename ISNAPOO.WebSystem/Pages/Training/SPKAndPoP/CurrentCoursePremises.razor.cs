using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CurrentCoursePremises : BlazorBaseComponent
    {
        private SfGrid<PremisesCourseVM> premisesGrid = new SfGrid<PremisesCourseVM>();
        private SfGrid<CandidateProviderPremisesRoomVM> premisesRoomsGrid = new SfGrid<CandidateProviderPremisesRoomVM>();
        private SelectCoursePremisesModal selectCoursePremisesModal = new SelectCoursePremisesModal();

        private List<PremisesCourseVM> premisesSource = new List<PremisesCourseVM>();
        private PremisesCourseVM premisesCourseToDelete = new PremisesCourseVM();

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.premisesSource = (await this.TrainingService.GetAllPremisesCoursesByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
        }

        private async Task AddPremisesBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.selectCoursePremisesModal.OpenModal(this.CourseVM, this.premisesSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeletePremisesBtn(PremisesCourseVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.premisesCourseToDelete = model;

                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
                if (isConfirmed)
                {
                   var result = new ResultContext<PremisesCourseVM>();
                    result.ResultContextObject = model;
                    result = await this.TrainingService.DeletePremisesCourseAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.premisesSource = (await this.TrainingService.GetAllPremisesCoursesByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
                        await this.premisesGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterPremisesSelectAsync(Dictionary<int, List<CandidateProviderPremisesVM>> selectedPremises)
        {
            this.SpinnerShow();

            var resultContext = new ResultContext<List<CandidateProviderPremisesVM>>();
            resultContext.ResultContextObject = selectedPremises.FirstOrDefault().Value;

            resultContext = await this.TrainingService.CreateTrainingCoursePremisesByListCandidateProviderPremisesVMAsync(resultContext, this.CourseVM.IdCourse, selectedPremises.FirstOrDefault().Key);
            if (resultContext.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                return;
            }

            await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

            this.premisesSource = (await this.TrainingService.GetAllPremisesCoursesByIdCourseAsync(this.CourseVM.IdCourse)).ToList();

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
                ExportProperties.FileName = $"CoursePremisesList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.premisesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CoursePremisesList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.premisesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "TrainingTypeName", HeaderText = "Вид на провежданото обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.PremisesName", HeaderText = "Материално-техническа база", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.Location.LocationName", HeaderText = "Населено място", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.ProviderAddress", HeaderText = "Адрес", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.Phone", HeaderText = "Телефон", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
