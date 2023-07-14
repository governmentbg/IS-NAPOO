using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseCurriculumSchedule : BlazorBaseComponent
    {
        private SfGrid<CourseScheduleVM> curriculumScheduleGrid = new SfGrid<CourseScheduleVM>();
        private CourseCurriculumScheduleModal courseCurriculumScheduleModal = new CourseCurriculumScheduleModal();
        private SelectSchedulePremisesModal selectSchedulePremisesModal = new SelectSchedulePremisesModal();
        private SelectScheduleTrainerModal selectScheduleTrainerModal = new SelectScheduleTrainerModal();
        private ImportCourseScheduleModal importCourseScheduleModal = new ImportCourseScheduleModal();

        private List<CourseScheduleVM> curriculumScheduleSource = new List<CourseScheduleVM>();
        private CourseScheduleVM scheduleToDelete = new CourseScheduleVM();
        private int idKVTheory = 0;
        private int idKVPractice = 0;

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool IsEditable { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await this.LoadScheduleDataAsync();
            await this.LoadNomenclaturesDataAsync();
        }

        private async Task LoadScheduleDataAsync()
        {
            this.curriculumScheduleSource = (await this.TrainingService.GetCourseSchedulesBydIdCourseAsync(this.CourseVM.IdCourse)).ToList();
        }

        private async Task LoadNomenclaturesDataAsync()
        {
            this.idKVPractice = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TrainingScheduleType", "Practice")).IdKeyValue;
            this.idKVTheory = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TrainingScheduleType", "Theory")).IdKeyValue;
        }

        private async Task AddScheduleBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.courseCurriculumScheduleModal.OpenModal(new CourseScheduleVM() { TrainingCurriculum = new TrainingCurriculumVM() }, this.curriculumScheduleSource, this.CourseVM, this.CourseVM.Program.IdSpeciality);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditScheduleBtn(CourseScheduleVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var courseSchedule = await this.TrainingService.GetCourseScheduleByIdAsync(model.IdCourseSchedule);
                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(courseSchedule.IdCourseSchedule, "CourseSchedule");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(courseSchedule.IdCourseSchedule, "CourseSchedule");
                }

                await this.courseCurriculumScheduleModal.OpenModal(courseSchedule, this.curriculumScheduleSource, this.CourseVM, this.CourseVM.Program.IdSpeciality, concurrencyInfoValue);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteScheduleBtn(CourseScheduleVM model)
        {
            this.scheduleToDelete = model;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteCourseScheduleByIdAsync(model.IdCourseSchedule);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    await this.LoadScheduleDataAsync();

                    await this.curriculumScheduleGrid.Refresh();
                    this.StateHasChanged();
                }
            }
        }

        private async Task UpdateAfterCurriculumScheduleModalSubmit()
        {
            await this.LoadScheduleDataAsync();

            await this.curriculumScheduleGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task AddMTBBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedRows = await this.curriculumScheduleGrid.GetSelectedRecordsAsync();
                if (!selectedRows.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне един ред от списъка с теми!");
                    this.SpinnerHide();
                    return;
                }

                if (selectedRows.Count > 1)
                {
                    foreach (var topic in selectedRows)
                    {
                        if (selectedRows.Any(x => x.IdTrainingScheduleType != topic.IdTrainingScheduleType))
                        {
                            await this.ShowErrorAsync("Моля, изберете теми от един и същи вид на проведеното обучение!");
                            this.SpinnerHide();
                            return;
                        }
                    }
                }

                var idTrainingType = selectedRows.FirstOrDefault()!.IdTrainingScheduleType;
                var premises = (await this.TrainingService.GetPremisesByIdTrainingTypeByIdCourseAsync(idTrainingType, this.CourseVM.IdCourse)).ToList();
                if (!premises.Any())
                {
                    await this.ShowErrorAsync("Няма данни за добавени МТБ към курса!");
                    this.SpinnerHide();
                    return;
                }

                var scheduleIds = selectedRows.Select(x => x.IdCourseSchedule).ToList();
                this.selectSchedulePremisesModal.OpenModal(scheduleIds, premises);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddTrainerBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedRows = await this.curriculumScheduleGrid.GetSelectedRecordsAsync();
                if (!selectedRows.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне един ред от списъка с теми!");
                    this.SpinnerHide();
                    return;
                }

                if (selectedRows.Count > 1)
                {
                    foreach (var topic in selectedRows)
                    {
                        if (selectedRows.Any(x => x.IdTrainingScheduleType != topic.IdTrainingScheduleType))
                        {
                            await this.ShowErrorAsync("Моля, изберете теми от един и същи вид на проведеното обучение!");
                            this.SpinnerHide();
                            return;
                        }
                    }
                }

                var idTrainingType = selectedRows.FirstOrDefault()!.IdTrainingScheduleType;
                var trainers = (await this.TrainingService.GetTrainersByIdTrainingTypeByIdCourseAsync(idTrainingType, this.CourseVM.IdCourse)).ToList();
                if (!trainers.Any())
                {
                    await this.ShowErrorAsync("Няма данни за добавени преподаватели към курса!");
                    this.SpinnerHide();
                    return;
                }

                var scheduleIds = selectedRows.Select(x => x.IdCourseSchedule).ToList();
                this.selectScheduleTrainerModal.OpenModal(scheduleIds, trainers);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterSelectModalSubmit(ResultContext<NoResult> resultContext)
        {
            if (resultContext.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join("", resultContext.ListErrorMessages));
                return;
            }

            await this.LoadScheduleDataAsync();

            await this.curriculumScheduleGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task UpdateAfterImportModalSubmit()
        {
            await this.LoadScheduleDataAsync();

            await this.curriculumScheduleGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task PrintSchedulePlanBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var clients = await this.TrainingService.GetCourseClientsByIdCourseAsync(this.CourseVM.IdCourse);
                var result = await this.TrainingService.PrintSchedulePlanAsync(this.curriculumScheduleSource, this.CourseVM.CandidateProvider, clients.ToList(), this.CourseVM.Program.IdSpeciality, this.CourseVM.StartDate);
                await FileUtils.SaveAs(this.JsRuntime, "Dnevnik_za_kvalifikacionen_kurs.docx", result.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task PrintScheduleProfessionalTrainingBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var clients = await this.TrainingService.GetCourseClientsByIdCourseAsync(this.CourseVM.IdCourse);
                var result = await this.TrainingService.PrintScheduleProfessionalTrainingAsync(this.CourseVM);
                await FileUtils.SaveAs(this.JsRuntime, "Grafik_na_profesionalnoto_obuchenie.docx", result.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task ScheduleTemplateDownloadBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var excelStream = await this.TrainingService.GetCourseScheduleTemplateWithCurriculumsFilledByIdCourseAsync(this.CourseVM.IdCourse);
                await this.JsRuntime.SaveAs($"Dnevnik_ucheben_plan_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", excelStream.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task ImportScheduleBtn()
        {
            if (this.curriculumScheduleSource.Any())
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Вече има въведени данни за дневник на обучение. Ако желаете да импортирате, данните ще бъдат презаписани. Сигурни ли сте, че искате да продължите?");
                if (isConfirmed)
                {
                    this.SpinnerShow();

                    if (this.loading)
                    {
                        return;
                    }
                    try
                    {
                        this.loading = true;

                        await this.importCourseScheduleModal.OpenModal(this.CourseVM.IdCourse);
                    }
                    finally
                    {
                        this.loading = false;
                    }

                    this.SpinnerHide();
                }
            }
            else
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    await this.importCourseScheduleModal.OpenModal(this.CourseVM.IdCourse);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task DeleteSelectedSchedulesBtn()
        {
            var selectedSchedules = await this.curriculumScheduleGrid.GetSelectedRecordsAsync();
            if (!selectedSchedules.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред с тема от дневника за изтриване!");
                return;
            }

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избраните записи?");
            if (isConfirmed)
            {
                this.SpinnerShow();

                var result = await this.TrainingService.DeleteListCurriculumSchedulesAsync(selectedSchedules);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                    await this.LoadScheduleDataAsync();
                    this.StateHasChanged();
                }

                this.SpinnerHide();
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
                ExportProperties.FileName = $"Course_Curriculum_ScheduleList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.curriculumScheduleGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Course_Curriculum_ScheduleList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.curriculumScheduleGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ScheduleDateAsStr", HeaderText = "Дата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TrainingCurriculum.ProfessionalTraining", HeaderText = "Раздел", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TrainingCurriculum.Topic", HeaderText = "Тема", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TrainingCurriculum.Subject", HeaderText = "Предмет", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TrainingScheduleType", HeaderText = "Вид", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Hours", HeaderText = "Часове", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Period", HeaderText = "Продължителност (от-до)", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.PremisesName", HeaderText = "МТБ", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.FullName", HeaderText = "Преподавател", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
