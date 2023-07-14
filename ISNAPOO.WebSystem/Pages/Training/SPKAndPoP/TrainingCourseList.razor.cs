using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class TrainingCourseList : BlazorBaseComponent
    {
        private SfGrid<CourseVM> coursesGrid = new SfGrid<CourseVM>();
        private UpcomingTrainingCourseModal upcomingTrainingCourseModal = new UpcomingTrainingCourseModal();
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();
        private CourseImportModal courseImportModal = new CourseImportModal();

        TrainingCourseListFilterModal filterModal;
        private List<CourseVM> coursesSource = new List<CourseVM>();
        private string title = string.Empty;
        private string type = string.Empty;
        private CourseVM courseToDelete = new CourseVM();
        private IEnumerable<KeyValueVM> formEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> typeFrameworkSource = new List<KeyValueVM>();
        private KeyValueVM kvPartOfProfession = new KeyValueVM();
        private KeyValueVM kvSPK = new KeyValueVM();
        private bool hideAddCourseBtn = false;
        private bool hideImportBtn = false;
        private int idCourseType = 0;
        private bool hideDeleteBtn = false;
        private bool showRIDPKCountColumns = false;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.formEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
            this.typeFrameworkSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
            this.kvPartOfProfession = this.typeFrameworkSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession");
            this.kvSPK = this.typeFrameworkSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification");

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
                case GlobalConstants.UPCOMING_COURSES_SPK:
                    this.hideImportBtn = true;
                    this.idCourseType = this.kvSPK.IdKeyValue;
                    this.title = "Предстоящи курсове за обучение за СПК";
                    this.coursesSource = (await this.TrainingService.GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    break;
                case GlobalConstants.CURRENT_COURSES_SPK:
                    this.idCourseType = this.kvSPK.IdKeyValue;
                    this.coursesSource = (await this.TrainingService.GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    this.hideAddCourseBtn = true;
                    this.hideImportBtn = false;
                    this.title = "Текущи курсове за обучение за СПК";
                    this.hideDeleteBtn = true;
                    break;
                case GlobalConstants.COMPLETED_COURSES_SPK:
                    this.idCourseType = this.kvSPK.IdKeyValue;
                    this.hideAddCourseBtn = true;
                    this.hideImportBtn = true;
                    this.title = "Приключили курсове за обучение за СПК";
                    this.hideDeleteBtn = true;
                    this.coursesSource = (await this.TrainingService.GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    await this.TrainingService.SetRIDPKCountAsync(this.coursesSource);
                    break;
                case GlobalConstants.ARCHIVED_COURSES_SPK:
                    this.title = "Архивирани курсове за обучение за СПК";
                    this.hideImportBtn = true;
                    this.idCourseType = this.kvSPK.IdKeyValue;
                    this.hideAddCourseBtn = true;
                    this.hideDeleteBtn = true;
                    this.coursesSource = (await this.TrainingService.GetAllArchivedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    await this.TrainingService.SetRIDPKCountAsync(this.coursesSource);
                    break;
                case GlobalConstants.UPCOMING_COURSES_PP:
                    this.title = "Предстоящи курсове за обучение за част от професия";
                    this.hideImportBtn = true;
                    this.idCourseType = this.kvPartOfProfession.IdKeyValue;
                    this.coursesSource = (await this.TrainingService.GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    break;
                case GlobalConstants.CURRENT_COURSES_PP:
                    this.title = "Текущи курсове за обучение за част от професия";
                    this.hideImportBtn = false;
                    this.idCourseType = this.kvPartOfProfession.IdKeyValue;
                    this.hideAddCourseBtn = true;
                    this.hideDeleteBtn = true;
                    this.coursesSource = (await this.TrainingService.GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    break;
                case GlobalConstants.COMPLETED_COURSES_PP:
                    this.title = "Приключили курсове за обучение за част от професия";
                    this.idCourseType = this.kvPartOfProfession.IdKeyValue;
                    this.hideAddCourseBtn = true;
                    this.hideImportBtn = true;
                    this.hideDeleteBtn = true;
                    this.coursesSource = (await this.TrainingService.GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    break;
                default:
                    this.title = "Архивирани курсове за обучение за част от професия";
                    this.hideImportBtn = true;
                    this.idCourseType = this.kvPartOfProfession.IdKeyValue;
                    this.hideAddCourseBtn = true;
                    this.hideDeleteBtn = true;
                    this.coursesSource = (await this.TrainingService.GetAllArchivedTrainingCoursesByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();
                    break;
            }

            // проверява дали курсовете са приключили или ахривирани и дали са за СПК
            this.showRIDPKCountColumns = this.idCourseType == this.kvSPK.IdKeyValue && (this.type == GlobalConstants.COMPLETED_COURSES_SPK || this.type == GlobalConstants.ARCHIVED_COURSES_SPK);

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

                //var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(course.IdCourse);
                //var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(course.IdCourse, "TrainingCourse");
                //if (concurrencyInfoValue == null)
                //{
                //    await this.AddEntityIdAsCurrentlyOpened(course.IdCourse, "TrainingCourse");
                //}

                //if (this.type == GlobalConstants.UPCOMING_COURSES_SPK || this.type == GlobalConstants.UPCOMING_COURSES_PP)
                //{
                //    await this.upcomingTrainingCourseModal.OpenModal(courseFromDb, this.formEducationSource, this.idCourseType, concurrencyInfoValue);
                //}
                //else if (this.type == GlobalConstants.CURRENT_COURSES_SPK || this.type == GlobalConstants.CURRENT_COURSES_PP)
                //{
                //    await this.currentTrainingCourseModal.OpenModal(courseFromDb, this.type, concurrencyInfoValue);
                //}
                //else if (this.type == GlobalConstants.COMPLETED_COURSES_SPK || this.type == GlobalConstants.COMPLETED_COURSES_PP)
                //{
                //    var isDocumentPresent = await this.TrainingService.IsAnyClientWithDocumentPresentByIdCourseAsync(courseFromDb.IdCourse);
                //    await this.currentTrainingCourseModal.OpenModal(courseFromDb, this.type, concurrencyInfoValue, !isDocumentPresent);
                //}
                //else if (this.type == GlobalConstants.ARCHIVED_COURSES_SPK || this.type == GlobalConstants.ARCHIVED_COURSES_PP)
                //{
                //    await this.currentTrainingCourseModal.OpenModal(courseFromDb, this.type, concurrencyInfoValue, false);
                //}

                ConcurrencyInfo concurrencyInfoValue = null;
                if (this.GetUserRoles().Any(x => x.Contains("CPO")))
                {
                    concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(course.IdCourse, "TrainingCourse");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(course.IdCourse, "TrainingCourse");
                    }
                }

                if (this.type == GlobalConstants.UPCOMING_COURSES_SPK || this.type == GlobalConstants.UPCOMING_COURSES_PP)
                {
                    await this.upcomingTrainingCourseModal.OpenModal(course, this.formEducationSource, this.idCourseType, concurrencyInfoValue);
                }
                else if (this.type == GlobalConstants.CURRENT_COURSES_SPK || this.type == GlobalConstants.CURRENT_COURSES_PP)
                {
                    await this.currentTrainingCourseModal.OpenModal(course, concurrencyInfoValue);
                }
                else if (this.type == GlobalConstants.COMPLETED_COURSES_SPK || this.type == GlobalConstants.COMPLETED_COURSES_PP)
                {
                    var isDocumentPresent = await this.TrainingService.IsAnyClientWithDocumentPresentByIdCourseAsync(course.IdCourse);
                    await this.currentTrainingCourseModal.OpenModal(course, concurrencyInfoValue, !isDocumentPresent);
                }
                else if (this.type == GlobalConstants.ARCHIVED_COURSES_SPK || this.type == GlobalConstants.ARCHIVED_COURSES_PP)
                {
                    await this.currentTrainingCourseModal.OpenModal(course, concurrencyInfoValue, false);
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

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
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
            if (!this.showRIDPKCountColumns)
            {
                ExportColumns.Add(new GridColumn() { Field = "FormEducationName", HeaderText = "Форма на обучение", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "MandatoryHours", HeaderText = "Задължителни уч. ч.", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SelectableHours", HeaderText = "Избираеми уч. ч.", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SubscribeDateAsStr", HeaderText = "Крайна дата за записване", TextAlign = TextAlign.Left });
            }

            ExportColumns.Add(new GridColumn() { Field = "StartDateAsStr", HeaderText = "Дата за започване на курса", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "EndDateAsStr", HeaderText = "Дата за завършване на курса", TextAlign = TextAlign.Left });
            if (this.showRIDPKCountColumns)
            {
                ExportColumns.Add(new GridColumn() { Field = "RIDPKCountSubmitted", HeaderText = "Подадени", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RIDPKCountReturned", HeaderText = "Върнати", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RIDPKCountEnteredInRegister", HeaderText = "Публикувани", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RIDPKCountDeclined", HeaderText = "Отказани", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RIDPKCountNotSubmitted", HeaderText = "Неподадени", TextAlign = TextAlign.Left });
            }

            return ExportColumns;
        }

        public async Task FilterGrid()
        {
            await filterModal.openModal(type);
        }

        public async Task Filter(CourseVM filter)
        {

            coursesSource = (await this.TrainingService.filterCourses(filter, this.type, this.UserProps.IdCandidateProvider)).Where(x => x.IdTrainingCourseType == this.idCourseType).ToList();
            foreach (var course in this.coursesSource)
            {
                var formEducation = this.formEducationSource.FirstOrDefault(x => x.IdKeyValue == course.IdFormEducation);
                if (formEducation is not null)
                {
                    course.FormEducationName = formEducation.Name;
                }
            }
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
