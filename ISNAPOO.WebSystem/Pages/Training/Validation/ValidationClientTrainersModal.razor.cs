using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ValidationClientTrainersModal : BlazorBaseComponent
    {
        private SfGrid<ValidationTrainerVM> trainersGrid = new SfGrid<ValidationTrainerVM>();
        private SelectValidationTrainerModal selectValidationTrainerModal = new SelectValidationTrainerModal();
        private List<ValidationTrainerVM> trainersSource = new List<ValidationTrainerVM>();
        private ValidationTrainerVM trainerCourseToDelete = new ValidationTrainerVM();

        [Parameter]
        public ValidationClientVM ClientVM { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            trainersSource = (await TrainingService.GetAllValidationTrainersByIdValidationClientAsync(ClientVM.IdValidationClient)).ToList();
        }

        private async Task AddTrainerBtn()
        {
            SpinnerShow();

            if (loading)
            {
                return;
            }
            try
            {
                loading = true;

                await selectValidationTrainerModal.OpenModal(ClientVM, trainersSource);
            }
            finally
            {
                loading = false;
            }

            SpinnerHide();
        }

        private async Task DeleteTrainerBtn(ValidationTrainerVM model)
        {
            SpinnerShow();

            if (loading)
            {
                return;
            }
            try
            {
                loading = true;

                trainerCourseToDelete = model;

                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
                if (isConfirmed)
                {
                    var result = new ResultContext<ValidationTrainerVM>();
                    result.ResultContextObject = model;
                    result = await TrainingService.DeleteValidationTrainerAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await ShowSuccessAsync(string.Join("", result.ListMessages));

                        trainersSource = (await TrainingService.GetAllValidationTrainersByIdValidationClientAsync(ClientVM.IdValidationClient)).ToList();
                        await trainersGrid.Refresh();
                        StateHasChanged();
                    }
                }
            }
            finally
            {
                loading = false;
            }

            SpinnerHide();
        }

        private async Task UpdateAfterTrainerSelectAsync(Dictionary<int, List<CandidateProviderTrainerVM>> selectedTrainers)
        {
            SpinnerShow();

            var resultContext = new ResultContext<List<CandidateProviderTrainerVM>>();
            resultContext.ResultContextObject = selectedTrainers.FirstOrDefault().Value;

            resultContext = await TrainingService.CreateValidationTrainerByListCandidateProviderTrainerVMAsync(resultContext, ClientVM.IdValidationClient, selectedTrainers.FirstOrDefault().Key);
            if (resultContext.HasErrorMessages)
            {
                await ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                return;
            }

            await ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

            trainersSource = (await TrainingService.GetAllValidationTrainersByIdValidationClientAsync(ClientVM.IdValidationClient)).ToList();
            SpinnerHide();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();

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
                ExportProperties.FileName = $"ValidationClientTrainersList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await trainersGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ValidationClientTrainersList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();
                await trainersGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "TrainingTypeName", HeaderText = "Вид на провежданото обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.Email", HeaderText = "E-mail адрес", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}

