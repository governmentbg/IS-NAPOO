using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ClientCourseSubjects : BlazorBaseComponent
    {
        private SfGrid<CourseSubjectVM> courseSubjectGrid = new SfGrid<CourseSubjectVM>();
        private ClientCourseSubjectGradeModal clientCourseSubjectGradeModal = new ClientCourseSubjectGradeModal();

        private IEnumerable<CourseSubjectVM> courseSubjectSource = new List<CourseSubjectVM>();
        private CourseSubjectVM model = new CourseSubjectVM();
        private IEnumerable<KeyValueVM> kvProfessionalTrainingSource = new List<KeyValueVM>();

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
            this.editContext = new EditContext(this.model);
            this.FormTitle = "Оценки";

            this.kvProfessionalTrainingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.courseSubjectSource = await this.TrainingService.GetAllCourseSubjectsByIdCourseAsync(this.CourseVM.IdCourse);
            foreach (var subject in this.courseSubjectSource)
            {
                var trainingType = this.kvProfessionalTrainingSource.FirstOrDefault(x => x.IdKeyValue == subject.IdProfessionalTraining);
                if (trainingType is not null)
                {
                    subject.ProfessionalTrainingName = trainingType.DefaultValue1!;
                }
            }
        }

        private async Task UpdateEnteredGradesAfterSaveAsync(int idCourseSubject)
        {
            if (idCourseSubject != 0)
            {
                var courseSubject = this.courseSubjectSource.FirstOrDefault(x => x.IdCourseSubject == idCourseSubject);
                if (courseSubject is not null)
                {
                    if (courseSubject.TheoryHours != 0)
                    {
                        courseSubject.EnteredTheoryGradesCount = await this.TrainingService.GetCourseSubjectEnteredTheoryGradesAsync(idCourseSubject);
                    }

                    if (courseSubject.PracticeHours != 0)
                    {
                        courseSubject.EnteredPracticeGradesCount = await this.TrainingService.GetCourseSubjectEnteredPracticeGradesAsync(idCourseSubject);
                    }
                }
            }
        }

        private async Task EditStudentsGradesBtn(CourseSubjectVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.clientCourseSubjectGradeModal.OpenModal(model, courseSubjectSource.ToList());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void CustomizeCellHours(QueryCellInfoEventArgs<CourseSubjectVM> args)
        {
            if (args.Column.Field == "TheoryHours")
            {
                args.Cell.AddClass(new string[] { "cell-orange" });
            }

            if (args.Column.Field == "PracticeHours")
            {
                args.Cell.AddClass(new string[] { "cell-bluegreen" });
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
                ExportProperties.FileName = $"Course_SubjectList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.courseSubjectGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Course_SubjectList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.courseSubjectGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ProfessionalTrainingName", HeaderText = "Р", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Subject", HeaderText = "Предмет", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TheoryHours", HeaderText = "Уч. часове теория", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "PracticeHours", HeaderText = "Уч. часове практика", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "EnteredTheoryGradesCount", HeaderText = "Въведени оценки теория", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "EnteredPracticeGradesCount", HeaderText = "Въведени оценки практика", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
