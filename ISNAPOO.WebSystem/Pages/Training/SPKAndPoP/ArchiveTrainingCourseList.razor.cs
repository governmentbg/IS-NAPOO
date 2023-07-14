using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ArchiveTrainingCourseList : BlazorBaseComponent
    {
        private SfGrid<CourseVM> coursesGrid = new SfGrid<CourseVM>();
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();

        private IEnumerable<CourseVM> coursesSource = new List<CourseVM>();
        private IEnumerable<KeyValueVM> formEducationSource = new List<KeyValueVM>();

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var courseTypes = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram", false, true);
                this.formEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
                this.coursesSource = await this.TrainingService.GetAllArchivedAndOutOfOrderTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);

                foreach (var course in this.coursesSource)
                {
                    var formEducation = this.formEducationSource.FirstOrDefault(x => x.IdKeyValue == course.IdFormEducation);
                    if (formEducation is not null)
                    {
                        course.FormEducationName = formEducation.Name;
                    }

                    if (course.IdTrainingCourseType.HasValue)
                    {
                        var type = courseTypes.FirstOrDefault(x => x.IdKeyValue == course.IdTrainingCourseType.Value);
                        if (type is not null)
                        {
                            course.ArchiveCourseValue = type.Name;
                        }
                    }
                }
                this.coursesSource = this.coursesSource.OrderByDescending(x => x.EndDate);
               // await this.coursesGrid.Refresh();
                this.StateHasChanged();
            }
        }

        private async Task ViewCourseBtn(CourseVM course)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(course.IdCourse, "TrainingCourse");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(course.IdCourse, "TrainingCourse");
                }

                await this.currentTrainingCourseModal.OpenModal(course, concurrencyInfoValue, false);
            }
            finally
            {
                this.loading = false;
            }

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
                ExportProperties.FileName = $"ArchiveTrainingCourseList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.coursesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ArchiveTrainingCourseList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.coursesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "CourseName", HeaderText = "Курс", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Location.LocationName", HeaderText = "Населено място", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FormEducationName", HeaderText = "Форма на обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "MandatoryHours", HeaderText = "Задължителни уч. ч.", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SelectableHours", HeaderText = "Избираеми уч. ч.", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SubscribeDateAsStr", HeaderText = "Крайна дата за записване", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "StartDateAsStr", HeaderText = "Дата за започване на курса", TextAlign = TextAlign.Left});
            ExportColumns.Add(new GridColumn() { Field = "EndDateAsStr", HeaderText = "Дата за завършване на курса", TextAlign = TextAlign.Left});
            ExportColumns.Add(new GridColumn() { Field = "ArchiveCourseValue", HeaderText = "Вид", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
