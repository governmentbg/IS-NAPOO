using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.Build.Framework;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientList : BlazorBaseComponent
    {

        private List<ValidationClientVM> validationClients;

        private SfGrid<ValidationClientVM> refGrid;

        private TrainingValidationClientModal validationModal;

        private string type = string.Empty;
        private int IdTypeCourse = 0;

        private KeyValueVM kvPartOfProfession = new KeyValueVM();

        private KeyValueVM kvSPK = new KeyValueVM();

        private List<KeyValueVM> typeFrameworkProgramSource = new List<KeyValueVM>();

        private string header;

        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public ICommonService commonService { get; set; }

        [Inject]
        public ITrainingService trainingService { get; set; }
        protected override async void OnInitialized()
        {
            this.validationClients = (await this.trainingService.getAllValidationClients()).ToList();
            this.typeFrameworkProgramSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).ToList();
            this.kvPartOfProfession = this.typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfPartOfProfession");
            this.kvSPK = this.typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");


            await this.HandleTokenData();

        }

        public async Task HandleTokenData()
        {
            header = string.Empty;
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.commonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();

                    await this.updateTable();
                }
                else
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }
        public async Task openNewModal()
        {
            validationModal.openModal(new ValidationClientVM(),IdTypeCourse, true);
        }
        
        public async Task SelectRow(ValidationClientVM client)
        {
            if (loading) return;

            try
            {
                loading = true;
                var currentClient = await trainingService.GetValidationClientByIdAsync(client.IdValidationClient);
                bool areValidationTabsEditable = true;
                if (currentClient is not null && currentClient.IsArchived)
                {
                    areValidationTabsEditable = false;
                }

                if (areValidationTabsEditable)
                {
                    var kvStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
                    if (currentClient is not null && currentClient.IdStatus == kvStatusFinished.IdKeyValue)
                    {
                        bool isRIDPKDocumentSubmittedOrEnteredInRegister = await this.trainingService.IsRIDPKDocumentSubmittedOrEnteredInRegisterByIdValidationClientAsync(currentClient.IdValidationClient);
                        if (isRIDPKDocumentSubmittedOrEnteredInRegister)
                        {
                            areValidationTabsEditable = false;
                        }
                    }
                }

                validationModal.openModal(currentClient, IdTypeCourse, areValidationTabsEditable);
            }
            finally
            {
                loading = false;
            }
        }

        public async Task updateTable()
        {
            switch(type)
            {
                case GlobalConstants.TRAINING_VALIDATION_SPK:
                    this.IdTypeCourse = this.kvSPK.IdKeyValue;
                    validationClients = (await this.trainingService.getAllValidationClients()).Where(x => x.IdCourseType == this.IdTypeCourse && x.IdCandidateProvider == UserProps.IdCandidateProvider).ToList();
                    header = "Валидиране на СПК";
                    break;
                case GlobalConstants.TRAINING_VALIDATION_PP:
                    this.IdTypeCourse = this.kvPartOfProfession.IdKeyValue;
                    validationClients = (await this.trainingService.getAllValidationClients()).Where(x => x.IdCourseType == this.IdTypeCourse && x.IdCandidateProvider == UserProps.IdCandidateProvider).ToList();
                    header = "Валидиране на част от професия";
                    break;
            }

            this.StateHasChanged();
            if(refGrid is not null)
            await this.refGrid.Refresh();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "roleGrid_pdfexport")
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
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
                ExportProperties.FileName = $"AnnualStudents_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".pdf";

                await this.refGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id == "roleGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"AnnualStudents_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.refGrid.ExportToExcelAsync(ExportProperties);
            }
        }
    }
}
