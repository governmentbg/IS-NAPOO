using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.LegalCapacity
{
    public partial class LegalCapacityTrainingProgramList : BlazorBaseComponent
    {
        private SfGrid<ProgramVM> programsGrid = new SfGrid<ProgramVM>();
        private LegalCapacityTrainingProgramModal trainingProgramModal = new LegalCapacityTrainingProgramModal();
        private LegalCapacityUpcomingTrainingCourseModal upcomingTrainingCourseModal = new LegalCapacityUpcomingTrainingCourseModal();

        private IEnumerable<ProgramVM> programsSource = new List<ProgramVM>();
        private ProgramVM programToDelete = new ProgramVM();
        private string title = string.Empty;
        private int idCourseType = 0;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.title = "Програми за обучение за правоспособност";
                var kvRegulation = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
                this.idCourseType = kvRegulation.IdKeyValue;

                await this.LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            this.programsSource = await this.TrainingService.GetAllActiveLegalCapacityProgramsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);

            this.programsSource = this.programsSource.OrderBy(x => x.OldId).ThenBy(x => x.Speciality.CodeAsIntForOrderBy).ThenBy(x => x.IdProgram).ToList();

            await this.programsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task AddCourseBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new CourseVM()
                {
                    IdCandidateProvider = this.UserProps.IdCandidateProvider,
                };

                var formEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");

                await this.upcomingTrainingCourseModal.OpenModal(model, formEducationSource, this.idCourseType);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddNewProgramBtn()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new ProgramVM()
                {
                    IdCandidateProvider = this.UserProps.IdCandidateProvider,
                    IdCourseType = this.idCourseType
                };

                await this.trainingProgramModal.OpenModal(model, this.idCourseType);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task EditProgramBtn(ProgramVM program)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(program.IdProgram, "LegalCapacityTrainingProgram");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(program.IdProgram, "TrainingProgram");
                }

                await this.trainingProgramModal.OpenModal(program, this.idCourseType, concurrencyInfoValue);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task DeleteProgramAsync(ProgramVM program)
        {
            this.programToDelete = program;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var result = await this.TrainingService.MarkProgramAsDeletedByIdProgramAsync(this.programToDelete.IdProgram);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                    await this.LoadDataAsync();
                }
            }
        }

        private async Task UpdateProgramsSourceAfterProgramModalSubmit()
        {
            await this.LoadDataAsync();
        }

        private async Task UpdateHoursAfterCurriculumSubmit()
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
                ExportProperties.FileName = $"TrainingProgramList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.programsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"TrainingProgramList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.programsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "Speciality.Profession.CodeAndName", HeaderText = "Професия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Speciality.CodeAndName", HeaderText = "Специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ProgramName", HeaderText = "Наименование на програмата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ProgramNumber", HeaderText = "Номер на програмата", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
