using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
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
    public partial class TrainingCourseDuplicateList : BlazorBaseComponent
    {
        private SfGrid<ClientCourseDocumentVM> duplicatesGrid = new SfGrid<ClientCourseDocumentVM>();
        private TrainingCourseDuplicateIssueModal trainingCourseDuplicateIssueModal = new TrainingCourseDuplicateIssueModal();
        private CurrentCourseClientModal currentCourseClientModal = new CurrentCourseClientModal();

        private List<ClientCourseDocumentVM> duplicatesSource = new List<ClientCourseDocumentVM>();
        private string title = string.Empty;
        private IEnumerable<KeyValueVM> typeFrameworkSource = new List<KeyValueVM>();
        private KeyValueVM kvPartOfProfession = new KeyValueVM();
        private KeyValueVM kvSPK = new KeyValueVM();
        private string type = string.Empty;
        private int idCourseType = 0;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
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

                    await this.LoadDataAsync();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private async Task LoadDataAsync()
        {
            switch (this.type)
            {
                case GlobalConstants.COURSE_DUPLICATES_SPK:
                    this.idCourseType = this.kvSPK.IdKeyValue;
                    this.title = "Издаване на дубликати на свидетелства от курсове за обучение за СПК";
                    break;
                case GlobalConstants.COURSE_DUPLICATES_PP:
                    this.idCourseType = this.kvPartOfProfession.IdKeyValue;
                    this.title = "Издаване на дубликати на удостоверения от курсове за обучение за част от професия";
                    break;
            }

            this.duplicatesSource = (await this.TrainingService.GetAllIssuedDuplicatesFromCoursesByIdCandidateProviderAndByIdCourseTypeAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();

            this.StateHasChanged();
        }

        private async Task AddDuplicateBtn()
        {
            bool hasPermission = await CheckUserActionPermission("ManageTrainingCourseDuplicate", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.trainingCourseDuplicateIssueModal.OpenModal(this.type, this.idCourseType);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterDuplicateIssuedAsync()
        {
            await this.LoadDataAsync();
        }

        private async Task ViewClientBtn(ClientCourseDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var clientCourse = await this.TrainingService.GetTrainingClientCourseByIdAsync(model.ClientCourse.IdClientCourse);
                var courseStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus");
                await this.currentCourseClientModal.OpenModal(clientCourse, model.ClientCourse.Course, new List<ClientCourseVM>(), courseStatusSource, true, null, false);
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
                ExportProperties.FileName = $"Spisak_izdavane_na_dublikati_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.duplicatesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Spisak_izdavane_na_dublikati_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.duplicatesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Indent", HeaderText = "ЕГН/ЛНЧ/ИДН", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.CourseName", HeaderText = "Курс", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Period", HeaderText = "Период на обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.Profession.ComboBoxName", HeaderText = "Професия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.ComboBoxName", HeaderText = "Специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentRegNo", HeaderText = "Рег. номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentDateAsStr", HeaderText = "Дата на издаване", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
