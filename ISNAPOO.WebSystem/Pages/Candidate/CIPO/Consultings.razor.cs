using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class Consultings : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderConsultingVM> consultingsGrid = new SfGrid<CandidateProviderConsultingVM>();

        private IEnumerable<CandidateProviderConsultingVM> consultingsSource = new List<CandidateProviderConsultingVM>();
        private List<KeyValueVM> kvConsultingTypeSource = new List<KeyValueVM>();
        private int idConsultingType = 0;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool DisableFieldsWhenOpenFromProfile { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Parameter]
        public bool DisableFieldsWhenApplicationStatusIsNotDocPreparation { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsNAPOO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenActiveLicenceChange { get; set; }

        [Parameter]
        public bool IsUserProfileAdministrator { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.FormTitle = "Услуги";

                await this.LoadConsultingsDataAsync();

                await this.CheckForAlreadyAddedConsultingTypesAsync();

                await this.consultingsGrid.Refresh();
                this.StateHasChanged();
            }
        }

        private async Task LoadConsultingsDataAsync()
        {
            this.consultingsSource = await this.CandidateProviderService.GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
        }

        private async Task CheckForAlreadyAddedConsultingTypesAsync()
        {
            this.kvConsultingTypeSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType")).ToList();
            if (this.consultingsSource.Any())
            {
                foreach (var consulting in this.consultingsSource)
                {
                    this.kvConsultingTypeSource.RemoveAll(x => x.IdKeyValue == consulting.IdConsultingType);
                }
            }
        }

        private async Task AddConsultingTypeBtn()
        {
            if (this.idConsultingType == 0)
            {
                await this.ShowErrorAsync("Моля, изберете вид на услугата!");
                return;
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var candidateProviderConsultingVM = new CandidateProviderConsultingVM()
                {
                    IdCandidateProvider = this.CandidateProviderVM.IdCandidate_Provider,
                    IdConsultingType = this.idConsultingType
                };

                var result = await this.CandidateProviderService.CreateCandidateProviderConsultingAsync(candidateProviderConsultingVM);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    this.idConsultingType = 0;

                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                    await this.LoadConsultingsDataAsync();
                    await this.CheckForAlreadyAddedConsultingTypesAsync();
                    await this.consultingsGrid.Refresh();

                    this.StateHasChanged();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteConsultingTypeBtn(CandidateProviderConsultingVM model)
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    var result = await this.CandidateProviderService.DeleteCandidateProviderConsultingByIdAsync(model.IdCandidateProviderConsulting);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        await this.LoadConsultingsDataAsync();
                        await this.CheckForAlreadyAddedConsultingTypesAsync();

                        await this.consultingsGrid.Refresh();
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
                ExportProperties.FileName = $"ConsultingList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.consultingsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ConsultingList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.consultingsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ConsultingTypeValue", HeaderText = "Вид на услугата", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
