using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.LegalCapacity
{
    public partial class LegalCapacityTrainingCourseList : BlazorBaseComponent
    {
        private SfGrid<CourseVM> coursesGrid = new SfGrid<CourseVM>();
        private LegalCapacityUpcomingTrainingCourseModal upcomingTrainingCourseModal = new LegalCapacityUpcomingTrainingCourseModal();
        private LegalCapacityCurrentTrainingCourseModal currentTrainingCourseModal = new LegalCapacityCurrentTrainingCourseModal();
        private CourseImportModal courseImportModal = new CourseImportModal();

        TrainingCourseListFilterModal filterModal;
        private List<CourseVM> coursesSource = new List<CourseVM>();
        private string title = string.Empty;
        private string type = string.Empty;
        private CourseVM courseToDelete = new CourseVM();
        private IEnumerable<KeyValueVM> formEducationSource = new List<KeyValueVM>();
        private bool hideAddCourseBtn = false;
        private bool hideImportBtn = false;
        private int idCourseType = 0;
        private bool hideDeleteBtn = false;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.formEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
            var kvRegulation = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
            this.idCourseType = kvRegulation!.IdKeyValue;

            await this.HandleTokenData();
        }

        private async Task HandleTokenData()
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();

                    await this.LoadTrainingCoursesDataAsync();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private async Task LoadTrainingCoursesDataAsync()
        {
            switch (this.type)
            {
                case GlobalConstants.UPCOMING_COURSES_LC:
                    this.hideImportBtn = true;
                    this.title = "Предстоящи курсове за обучение за правоспособност";
                    this.coursesSource = (await this.TrainingService.GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();
                    break;
                case GlobalConstants.CURRENT_COURSES_LC:
                    this.coursesSource = (await this.TrainingService.GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();
                    this.hideAddCourseBtn = true;
                    this.hideImportBtn = false;
                    this.hideDeleteBtn = true;
                    this.title = "Текущи курсове за обучение за правоспособност";
                    break;
                case GlobalConstants.COMPLETED_COURSES_LC:
                    this.hideAddCourseBtn = true;
                    this.hideImportBtn = true;
                    this.hideDeleteBtn = true;
                    this.title = "Приключили курсове за обучение за правоспособност";
                    this.coursesSource = (await this.TrainingService.GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();
                    break;
                case GlobalConstants.ARCHIVED_COURSES_LC:
                    this.title = "Архивирани курсове за обучение за правоспособност";
                    this.hideImportBtn = true;
                    this.hideAddCourseBtn = true;
                    this.hideDeleteBtn = true;
                    this.coursesSource = (await this.TrainingService.GetAllArchivedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).ToList();
                    break;
            }

            this.coursesSource = this.coursesSource.Where(x => x.IdTrainingCourseType == this.idCourseType).ToList();

            foreach (var course in this.coursesSource)
            {
                var formEducation = this.formEducationSource.FirstOrDefault(x => x.IdKeyValue == course.IdFormEducation);
                if (formEducation is not null)
                {
                    course.FormEducationName = formEducation.Name;
                }
            }

            await this.coursesGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task UpdateTrainingCoursesAfterUpcomingTrainingCoursesModalSubmitAsync()
        {
            await this.LoadTrainingCoursesDataAsync();
        }

        private async Task UpdateTrainingCoursesAfterCurrentTrainingCoursesModalSubmitAsync()
        {
            await this.LoadTrainingCoursesDataAsync();
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

                await this.upcomingTrainingCourseModal.OpenModal(model, this.formEducationSource, this.idCourseType);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditCourseBtn(CourseVM course)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(course.IdCourse);

                ConcurrencyInfo concurrencyInfoValue = null;
                if (this.GetUserRoles().Any(x => x.Contains("CPO")))
                {
                    concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(course.IdCourse, "TrainingCourse");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(course.IdCourse, "TrainingCourse");
                    }
                }

                if (this.type == GlobalConstants.UPCOMING_COURSES_LC)
                {
                    await this.upcomingTrainingCourseModal.OpenModal(courseFromDb, this.formEducationSource, this.idCourseType, concurrencyInfoValue);
                }
                else if (this.type == GlobalConstants.CURRENT_COURSES_LC)
                {
                    await this.currentTrainingCourseModal.OpenModal(courseFromDb, concurrencyInfoValue);
                }
                else if (this.type == GlobalConstants.COMPLETED_COURSES_LC)
                {
                    await this.currentTrainingCourseModal.OpenModal(courseFromDb, concurrencyInfoValue);
                }
                else if (this.type == GlobalConstants.ARCHIVED_COURSES_LC)
                {
                    await this.currentTrainingCourseModal.OpenModal(courseFromDb, concurrencyInfoValue);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteCourseBtn(CourseVM course)
        {
            this.courseToDelete = course;
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteTrainingCourseByIdAsync(course.IdCourse);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    await this.LoadTrainingCoursesDataAsync();
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
                ExportProperties.FileName = $"TrainingCourseList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.coursesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"TrainingCourseList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
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
            ExportColumns.Add(new GridColumn() { Field = "SubscribeDate", HeaderText = "Крайна дата за записване", TextAlign = TextAlign.Left, Format = "dd.MM.yyyy" });
            ExportColumns.Add(new GridColumn() { Field = "StartDate", HeaderText = "Дата за започване на курса", TextAlign = TextAlign.Left, Format = "dd.MM.yyyy" });
            ExportColumns.Add(new GridColumn() { Field = "EndDate", HeaderText = "Дата за завършване на курса", TextAlign = TextAlign.Left, Format = "dd.MM.yyyy" });

            return ExportColumns;
        }

        public async Task FilterGrid()
        {
            await filterModal.openModal(type);
        }

        public async Task Filter(CourseVM filter)
        {
            coursesSource = await this.TrainingService.filterCourses(filter, this.type, this.UserProps.IdCandidateProvider);
        }

        public void ImportCourseBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.courseImportModal.OpenModal();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
