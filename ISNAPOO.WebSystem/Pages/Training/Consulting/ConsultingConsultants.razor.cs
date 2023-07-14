using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Consulting
{
    public partial class ConsultingConsultants : BlazorBaseComponent
    {
        private SfGrid<ConsultingTrainerVM> trainersGrid = new SfGrid<ConsultingTrainerVM>();
        private SelectConsultingTrainerModal selectConsultingTrainerModal = new SelectConsultingTrainerModal();

        private List<ConsultingTrainerVM> trainersSource = new List<ConsultingTrainerVM>();
        private ConsultingTrainerVM consultingTrainerToDelete = new ConsultingTrainerVM();

        [Parameter]
        public ConsultingClientVM ConsultingClientVM { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.trainersSource = (await this.TrainingService.GetAllConsultingTrainersByIdConsultingClientAsync(this.ConsultingClientVM.IdConsultingClient)).ToList();
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

                await this.selectConsultingTrainerModal.OpenModal(this.ConsultingClientVM, this.trainersSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteTrainerBtn(ConsultingTrainerVM model)
        {
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
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
                    this.consultingTrainerToDelete = model;

                        var result = new ResultContext<ConsultingTrainerVM>();
                        result.ResultContextObject = model;
                        result = await this.TrainingService.DeleteConsultingTrainerAsync(result);
                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                            this.trainersSource = (await this.TrainingService.GetAllConsultingTrainersByIdConsultingClientAsync(this.ConsultingClientVM.IdConsultingClient)).ToList();
                            await this.trainersGrid.Refresh();
                            this.StateHasChanged();
                        }                   
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task UpdateAfterTrainerSelectAsync(List<CandidateProviderTrainerVM> selectedTrainers)
        {
            this.SpinnerShow();

            var resultContext = new ResultContext<List<CandidateProviderTrainerVM>>();
            resultContext.ResultContextObject = selectedTrainers.ToList();

            resultContext = await this.TrainingService.CreateConsultingTrainersByListCandidateProviderTrainerVMAsync(resultContext, this.ConsultingClientVM.IdConsultingClient);
            if (resultContext.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                return;
            }

            await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

            this.trainersSource = (await this.TrainingService.GetAllConsultingTrainersByIdConsultingClientAsync(this.ConsultingClientVM.IdConsultingClient)).ToList();

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
                ExportProperties.FileName = $"Consulting_Consultants{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.trainersGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Consulting_Consultants{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.trainersGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderTrainer.Email", HeaderText = "E-mail адрес", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
