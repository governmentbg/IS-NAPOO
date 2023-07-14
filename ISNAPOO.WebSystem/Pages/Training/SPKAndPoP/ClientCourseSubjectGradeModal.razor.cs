using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ClientCourseSubjectGradeModal : BlazorBaseComponent
    {
        private SfGrid<CourseSubjectGradeVM> clientsGrid = new SfGrid<CourseSubjectGradeVM>();

        private IEnumerable<CourseSubjectGradeVM> clientSource = new List<CourseSubjectGradeVM>();
        private List<CourseSubjectVM> addedCourseSubjects = new List<CourseSubjectVM>();
        private CourseSubjectVM courseSubjectVM = new CourseSubjectVM();
        private CourseSubjectGradeVM model = new CourseSubjectGradeVM();
        private ValidationMessageStore? messageStore;
        private List<string> validationMessages = new List<string>();
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;
        private int idCourseSubject = 0;
        private int nextId = 0;
        private int previousId = 0;
        private int currentIndex = 0;
        private bool isSwitchBtnClicked = false;

        [Parameter]
        public EventCallback<int> CallbackAfterGradeSaved { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(CourseSubjectVM courseSubject, List<CourseSubjectVM> addedCourseSubjects)
        {
            this.validationMessages.Clear();
            this.editContext = new EditContext(this.model);

            this.courseSubjectVM = courseSubject;
            this.addedCourseSubjects = addedCourseSubjects;
            this.currentIndex = this.addedCourseSubjects.IndexOf(this.courseSubjectVM);

            await this.LoadGrades();

            this.SetButtonsState();
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadGrades()
        {
            var clientCourseList = await this.TrainingService.GetCourseClientsByIdCourseWithoutIncludesAsync(this.courseSubjectVM.IdCourse);
            var clientCourseIds = clientCourseList.Select(x => x.IdClientCourse).ToList();
            this.clientSource = await this.TrainingService.GetClientCourseSubjectGradeListByClientCourseListIdsAndByIdCourseSubjectAsync(clientCourseIds, this.courseSubjectVM.IdCourseSubject);
            foreach (var client in this.clientSource)
            {
                if (client.TheoryGrade != null)
                {
                    client.TheoryGradeAsStr = client.TheoryGrade.Value.ToString("f2");
                }
                else
                {
                    if (!string.IsNullOrEmpty(client.TheoryGradeAsStr))
                    {
                        client.TheoryGradeAsStr = string.Empty;
                    }
                }

                if (client.PracticeGrade != null)
                {
                    client.PracticeGradeAsStr = client.PracticeGrade.Value.ToString("f2");
                }
                else
                {
                    if (!string.IsNullOrEmpty(client.PracticeGradeAsStr))
                    {
                        client.PracticeGradeAsStr = string.Empty;
                    }
                }
            }

            this.StateHasChanged();
        }

        private async Task SaveTheoryGradeAsync(CourseSubjectGradeVM model)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.model.TheoryGradeAsStr = model.TheoryGradeAsStr;

                if (!this.isSwitchBtnClicked)
                {
                    if (!this.IsTheoryGradeValid())
                    {
                        this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                        await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                    }
                    else
                    {
                        model.TheoryGrade = BaseHelper.ConvertToFloat(model.TheoryGradeAsStr, 2);
                        if (model.TheoryGrade is not null)
                        {
                            model.TheoryGradeAsStr = model.TheoryGrade.Value.ToString("f2");
                            model.TheoryGrade = Math.Round(model.TheoryGrade.Value, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            model.TheoryGrade = null;
                        }

                        var result = new ResultContext<CourseSubjectGradeVM>();
                        result.ResultContextObject = model;

                        result = await this.TrainingService.UpdateClientCourseSubjectGradeAsync(result, true);
                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                            await this.CallbackAfterGradeSaved.InvokeAsync(model.IdCourseSubject);
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task SavePracticeGradeAsync(CourseSubjectGradeVM model)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.model.PracticeGradeAsStr = model.PracticeGradeAsStr;

                if (!this.isSwitchBtnClicked)
                {
                    if (!this.IsPracticeGradeValid())
                    {
                        this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                        await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                    }
                    else
                    {
                        model.PracticeGrade = BaseHelper.ConvertToFloat(model.PracticeGradeAsStr, 2);
                        if (model.PracticeGrade is not null)
                        {
                            model.PracticeGradeAsStr = model.PracticeGrade.Value.ToString("f2");
                            model.PracticeGrade = Math.Round(model.PracticeGrade.Value, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            model.PracticeGrade = null;
                        }

                        var result = new ResultContext<CourseSubjectGradeVM>();
                        result.ResultContextObject = model;

                        result = await this.TrainingService.UpdateClientCourseSubjectGradeAsync(result, false);
                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                            await this.CallbackAfterGradeSaved.InvokeAsync(model.IdCourseSubject);
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }
        }

        private bool IsTheoryGradeValid()
        {
            this.validationMessages.Clear();

            if (!string.IsNullOrEmpty(this.model.TheoryGradeAsStr))
            {
                if (BaseHelper.ConvertToFloat(this.model.TheoryGradeAsStr, 2) == null)
                {
                    this.validationMessages.Add("Полето 'Оценка теория' може да бъде само число!");
                    return false;
                }
            }

            var value = BaseHelper.ConvertToFloat(this.model.TheoryGradeAsStr, 2);
            if (value < 2)
            {
                this.validationMessages.Add("Полето 'Оценка теория' не може да има стойност по-малка от 2.00!");
                return false;
            }

            if (value > 6)
            {
                this.validationMessages.Add("Полето 'Оценка теория' не може да има стойност по-голяма от 6.00!");
                return false;
            }

            if (value.ToString().Length > 4)
            {
                this.validationMessages.Add("Полето 'Оценка теория' не може да съдържа повече от 2 знака след десетичната запетая!");
                return false;
            }

            return true;
        }

        private bool IsPracticeGradeValid()
        {
            this.validationMessages.Clear();

            if (!string.IsNullOrEmpty(this.model.PracticeGradeAsStr))
            {
                if (BaseHelper.ConvertToFloat(this.model.PracticeGradeAsStr, 2) == null)
                {
                    this.validationMessages.Add("Полето 'Оценка практика' може да бъде само число!");
                    return false;
                }
            }

            var value = BaseHelper.ConvertToFloat(this.model.PracticeGradeAsStr, 2);
            if (value < 2)
            {
                this.validationMessages.Add("Полето 'Оценка практика' не може да има стойност по-малка от 2.00!");
                return false;
            }

            if (value > 6)
            {
                this.validationMessages.Add("Полето 'Оценка практика' не може да има стойност по-голяма от 6.00!");
                return false;
            }

            if (value.ToString().Length > 4)
            {
                this.validationMessages.Add("Полето 'Оценка практика' не може да съдържа повече от 2 знака след десетичната запетая!");
                return false;
            }

            return true;
        }

        private void SetIsSwitchBtnState()
        {
            this.isSwitchBtnClicked = false;
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
                ExportProperties.FileName = $"Course_Clients_Grades_List_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.clientsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Course_Clients_Grades_List_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.clientsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });

            if (this.courseSubjectVM.TheoryHours != 0)
            {
                ExportColumns.Add(new GridColumn() { Field = "TheoryGrade", HeaderText = "Оценка теория", TextAlign = TextAlign.Left, Format = "f2" });
            }

            if (this.courseSubjectVM.PracticeHours != 0)
            {
                ExportColumns.Add(new GridColumn() { Field = "PracticeGrade", HeaderText = "Оценка практика", TextAlign = TextAlign.Left, Format = "f2" });
            }

            return ExportColumns;
        }

        private async void PreviousCurriculum()
        {
            var id = this.idCourseSubject == 0 ? this.courseSubjectVM.IdCourseSubject : this.idCourseSubject;

            this.previousId = this.addedCourseSubjects.FindIndex(x => x.IdCourseSubject == id) - 1;
            if (this.previousId == -1)
            {
                this.previousId = this.addedCourseSubjects.Count - 1;
            }

            if (this.previousId >= 0)
            {
                this.courseSubjectVM = this.addedCourseSubjects[this.previousId];
                this.currentIndex = this.addedCourseSubjects.IndexOf(this.courseSubjectVM);
                this.SetButtonsState();
                await this.LoadGrades();

                this.isSwitchBtnClicked = true;

                this.StateHasChanged();
            }
        }

        private async void NextCurriculum()
        {
            this.nextId = this.addedCourseSubjects.FindIndex(x => x.IdCourseSubject == this.courseSubjectVM.IdCourseSubject) + 1;
            if (this.nextId < this.addedCourseSubjects.Count)
            {
                this.courseSubjectVM = this.addedCourseSubjects[this.nextId];
                this.currentIndex = this.addedCourseSubjects.IndexOf(this.courseSubjectVM);
                this.SetButtonsState();
                await this.LoadGrades();

                this.isSwitchBtnClicked = true;

                this.StateHasChanged();
            }
        }

        private void SetButtonsState()
        {
            this.nextId = currentIndex + 1;
            this.previousId = currentIndex - 1;

            if (this.nextId == this.addedCourseSubjects.Count)
            {
                this.disableNextBtn = true;
            }
            else
            {
                this.disableNextBtn = false;
            }

            if (this.previousId < 0)
            {
                this.disablePreviousBtn = true;
            }
            else
            {
                this.disablePreviousBtn = false;
            }
        }
    }
}
