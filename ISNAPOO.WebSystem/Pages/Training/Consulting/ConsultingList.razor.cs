using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Consulting
{
    public partial class ConsultingList : BlazorBaseComponent
    {
        private SfGrid<ConsultingClientVM> consultingClientsGrid = new SfGrid<ConsultingClientVM>();
        private ConsultingClientModal consultingClientModal = new ConsultingClientModal();

        private IEnumerable<ConsultingClientVM> consultingClientsSource = new List<ConsultingClientVM>();
        private ConsultingClientVM consultingClientToDelete = new ConsultingClientVM();
        private ConsultingListFilterModal filterModal;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadConsultingsClientsDataAsync();
        }

        private async Task LoadConsultingsClientsDataAsync()
        {
            this.consultingClientsSource = await TrainingService.GetAllConsultingClientsByIdCandidateProviderAsync(UserProps.IdCandidateProvider);

            StateHasChanged();
        }

        public async Task FilterGrid()
        {
            this.filterModal.OpenModal();
        }
        private async Task AddConsultingClientBtn()
        {
            SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.consultingClientModal.OpenModal(new ConsultingClientVM());
            }
            finally
            {
                this.loading = false;
            }

            SpinnerHide();
        }

        private async Task EditConsultingClientBtn(ConsultingClientVM model)
        {
            SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var consultingClient = await TrainingService.GetConsultingClientByIdAsync(model.IdConsultingClient);
                var concurrencyInfoValue = GetAllCurrentlyOpenedModalsConcurrencyInfoValue(consultingClient.IdConsultingClient, "Consulting");
                if (concurrencyInfoValue == null)
                {
                    await AddEntityIdAsCurrentlyOpened(consultingClient.IdConsultingClient, "Consulting");
                }

                await this.consultingClientModal.OpenModal(consultingClient);
            }
            finally
            {
                this.loading = false;
            }

            SpinnerHide();
        }

        private async Task DeleteConsultingClientBtn(ConsultingClientVM model)
        {     
            this.consultingClientToDelete = model;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {

                var result = await TrainingService.DeleteConsultingClientByIdAsync(model.IdConsultingClient);
                if (result.HasErrorMessages)
                {
                    await ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    await LoadConsultingsClientsDataAsync();
                }
            }
        }

        private async Task UpdateAfterConsultingClientModalSubmitAsync()
        {
            await LoadConsultingsClientsDataAsync();
        }
        public async Task Filter(ConsultingClientVM filter)
        {
            this.consultingClientsSource = (await TrainingService.FilterConsultingClients(filter, UserProps.IdCandidateProvider)).ToList();
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
                ExportProperties.FileName = $"TrainingCourseList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.consultingClientsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"TrainingCourseList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();
                await this.consultingClientsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "IndentType", HeaderText = "Вид на идентификатора", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Indent", HeaderText = "ЕГН/ИДН/ЛНЧ", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "StartDateAsStr", HeaderText = "Период на консултиране от", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "EndDateAsStr", HeaderText = "Период на консултиране до", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
