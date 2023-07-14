using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Pdf.Grid;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CurriculumModificationHistoryModal : BlazorBaseComponent
    {
        private SfGrid<CandidateCurriculumModificationVM> modificationsGrid = new SfGrid<CandidateCurriculumModificationVM>();
        private CurriculumModificationModal curriculumModificationModal = new CurriculumModificationModal();

        private IEnumerable<CandidateCurriculumModificationVM> modificationsSource = new List<CandidateCurriculumModificationVM>();
        private string title = string.Empty;
        private int idCandidateProviderSpeciality = 0;
        private CandidateProviderSpecialityVM candidateProviderSpecialityVM = new CandidateProviderSpecialityVM();
        private SpecialityVM specialityVM = new SpecialityVM();
        private bool hideAllActionsInCurriculumModal = true;

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService UploadService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public async Task OpenModal(CandidateProviderSpecialityVM candidateProviderSpeciality, SpecialityVM speciality)
        {
            this.idCandidateProviderSpeciality = candidateProviderSpeciality.IdCandidateProviderSpeciality;
            this.candidateProviderSpecialityVM = candidateProviderSpeciality;
            this.specialityVM = speciality;

            this.title = $"Данни за промени на учебен план и учебни програми за специалност <span style=\"color: #ffffff;\">{this.specialityVM.CodeAndName}</span>";

            await this.LoadDataAsync();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadDataAsync()
        {
            this.modificationsSource = await this.CandidateProviderService.GetAllCurriculumModificationsByIdCandidateProviderSpecialityAsync(this.idCandidateProviderSpeciality);
            this.StateHasChanged();
        }

        private async Task OpenCandidateCurriculumModalBtn(CandidateCurriculumModificationVM candidateCurriculumModification)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.curriculumModificationModal.OpenModal(candidateCurriculumModification.IdCandidateCurriculumModification, this.candidateProviderSpecialityVM, this.specialityVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DownloadCurriculumModificationFileBtn(CandidateCurriculumModificationVM candidateCurriculumModification)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadService.CheckIfExistUploadedFileAsync<CandidateCurriculumModification>(candidateCurriculumModification.IdCandidateCurriculumModification);
                if (hasFile)
                {
                    var documentStream = await this.UploadService.GetUploadedFileAsync<CandidateCurriculumModification>(candidateCurriculumModification.IdCandidateCurriculumModification);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, candidateCurriculumModification.FileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "modificationsGrid_pdfexport")
            {
                int temp = this.modificationsGrid.PageSettings.PageSize;
                modificationsGrid.PageSettings.PageSize = this.modificationsSource.Count();
                await modificationsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });               
                ExportColumns.Add(new GridColumn() { Field = "ModificationReasonValue", HeaderText = "Причина за промяна", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ValidFromDateAsStr", HeaderText = "В сила от", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ModificationStatusValue", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });               
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
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
                ExportProperties.FileName = $"CurriculumModificationHistory_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".pdf";

                await this.modificationsGrid.ExportToPdfAsync(ExportProperties);
                modificationsGrid.PageSettings.PageSize = temp;
                await modificationsGrid.Refresh();
            }
            else if (args.Item.Id == "modificationsGrid_excelexport")
            {
                this.modificationsGrid.AllowExcelExport = true;
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "ModificationReasonValue", HeaderText = "Причина за промяна", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ValidFromDateAsStr", HeaderText = "В сила от", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ModificationStatusValue", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"CurriculumModificationHistory_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.modificationsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<CandidateCurriculumModificationVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(modificationsGrid, args.Data.IdCandidateCurriculumModification).Result.ToString();
            }
        }
    }
}
