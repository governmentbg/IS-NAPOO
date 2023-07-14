using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.LegalCapacity;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseGraduatesList : BlazorBaseComponent
    {
        private SfGrid<ClientCourseVM> graduatesGrid = new SfGrid<ClientCourseVM>();
        private CurrentCourseClientModal currentCourseClientModal = new CurrentCourseClientModal();
        private LegalCapacityCurrentCourseClientModal legalCapacityCurrentCourseClientModal = new LegalCapacityCurrentCourseClientModal();

        private IEnumerable<ClientCourseVM> graduatesSource = new List<ClientCourseVM>();
        private string type = string.Empty;
        private bool isCourseSPK = false;
        private const string firstInitialFilterValue = "Завършил курса, но не положил изпит";
        private const string secondInitialFilterValue = "Завършил с документ";
        private const string thirdInitialFilterValue = "Прекъснал по уважителни причини";
        private const string fourthInitialFilterValue = "Прекъснал по неуважителни причини";
        private const string fifthInitialFilterValue = "Придобил СПК по реда на чл.40 от ЗПОО";
        private const string sixthInitialFilterValue = "Издаване на дубликат";
        private const string seventhInitialFilterValue = "Процедурата по валидиране не е завършила с издаване на документ";
        private const string eigthInitialFilterValue = "Придобил документ по реда на чл.40 от ЗПОО";

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        protected override async Task OnInitializedAsync()
        {
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

                    await this.LoadGraduatesDataAsync();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private async Task LoadGraduatesDataAsync()
        {
            int idCourseType;
            switch (this.type)
            {
                case GlobalConstants.COURSE_GRADUATES_SPK:
                    idCourseType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification")).IdKeyValue;
                    break;
                case GlobalConstants.COURSE_GRADUATES_LC:
                    idCourseType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7")).IdKeyValue;
                    break;
                default:
                    idCourseType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession")).IdKeyValue;
                    break;
            }

            this.graduatesSource = await this.TrainingService.GetAllClientCourseFromCoursesByIdCourseTypeAndIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, idCourseType);
        }

        private async Task UpdateAfterCourseClientModalSubmitAsync()
        {
            await this.LoadGraduatesDataAsync();

            await this.graduatesGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task EditClientCourseBtn(ClientCourseVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var clientCourse = await this.TrainingService.GetTrainingClientCourseByIdAsync(model.IdClientCourse);
                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(clientCourse.IdClientCourse, "TrainingClientCourse");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(clientCourse.IdClientCourse, "TrainingClientCourse");
                }

                var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(model.IdCourse);
                var clientsFromCourse = await this.TrainingService.GetCourseClientsByIdCourseAsync(model.IdCourse);
                var courseStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus");
                var isDocPresent = await this.TrainingService.IsDocumentPresentAsync(clientCourse.IdClientCourse);

                if (this.type == GlobalConstants.COURSE_GRADUATES_SPK)
                {
                    this.isCourseSPK = true;
                    await this.currentCourseClientModal.OpenModal(clientCourse, courseFromDb, clientsFromCourse.ToList(), courseStatusSource, isDocPresent, concurrencyInfoValue);
                }
                else if (this.type == GlobalConstants.COURSE_GRADUATES_LC) 
                {
                    await this.legalCapacityCurrentCourseClientModal.OpenModal(clientCourse, courseFromDb, clientsFromCourse.ToList(), courseStatusSource, isDocPresent, false, concurrencyInfoValue);
                }
                else if (this.type == GlobalConstants.COURSE_GRADUATES_PP)
                {
                    await this.currentCourseClientModal.OpenModal(clientCourse, courseFromDb, clientsFromCourse.ToList(), courseStatusSource, isDocPresent, concurrencyInfoValue);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task ToolbarClick(ClickEventArgs args)
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
                ExportProperties.FileName = $"CourseGraduatesList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.graduatesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CourseGraduatesList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.graduatesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "Course.CourseName", HeaderText = "Курс", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CoursePeriod", HeaderText = "Период на обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Course.Program.Speciality.Profession.CodeAndName", HeaderText = "Професия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Course.Program.Speciality.CodeAndAreaForAutoCompleteSearch", HeaderText = "Специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Indent", HeaderText = "ЕГН/ЛНЧ/ИДН", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FinishedTypeName", HeaderText = "Статус на завършване", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
