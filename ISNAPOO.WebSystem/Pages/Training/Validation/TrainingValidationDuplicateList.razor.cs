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

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationDuplicateList : BlazorBaseComponent
    {
        private SfGrid<ValidationClientDocumentVM> duplicatesGrid = new SfGrid<ValidationClientDocumentVM>();
        private TrainingValidationDuplicateIssueModal trainingValidationDuplicateIssueModal = new TrainingValidationDuplicateIssueModal();
        private TrainingValidationClientModal validationModal = new TrainingValidationClientModal();

        private List<ValidationClientDocumentVM> duplicatesSource = new List<ValidationClientDocumentVM>();
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
            this.kvPartOfProfession = this.typeFrameworkSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfPartOfProfession");
            this.kvSPK = this.typeFrameworkSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");

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
                case GlobalConstants.VALIDATION_DUPLICATES_SPK:
                    this.idCourseType = this.kvSPK.IdKeyValue;
                    this.title = "Издаване на дубликати на свидетелства от валидиране за СПК";
                    break;
                case GlobalConstants.VALIDATION_DUPLICATES_PP:
                    this.idCourseType = this.kvPartOfProfession.IdKeyValue;
                    this.title = "Издаване на дубликати на удостоверения от валидиране за част от професия";
                    break;
            }

            this.duplicatesSource = (await this.TrainingService.GetAllIssuedDuplicatesFromValidationsByIdCandidateProviderAndByIdCourseTypeAsync(this.UserProps.IdCandidateProvider, this.idCourseType)).ToList();

            this.StateHasChanged();
        }

        private async Task AddDuplicateBtn()
        {
            bool hasPermission = await CheckUserActionPermission("ManageTrainingValidationDuplicate", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.trainingValidationDuplicateIssueModal.OpenModal(this.type, this.idCourseType);
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

        private async Task ViewClientBtn(ValidationClientDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var currentClient = await this.TrainingService.GetValidationClientByIdAsync(model.ValidationClient.IdValidationClient);
                this.validationModal.openModal(currentClient, this.idCourseType, false);
            }
            finally
            {
                loading = false;
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

            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.Indent", HeaderText = "ЕГН/ЛНЧ/ИДН", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.Period", HeaderText = "Период на процедурата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.Speciality.Profession.ComboBoxName", HeaderText = "Професия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.Speciality.ComboBoxName", HeaderText = "Специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentRegNo", HeaderText = "Рег. номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentDateAsStr", HeaderText = "Дата на издаване", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
