using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.FrameworkProgram;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.SPPOO
{
    partial class FrameworkProgramList : BlazorBaseComponent
    {
        ToastMsg toast;
        SfGrid<FrameworkProgramVM> frameworkProgramGrid = new SfGrid<FrameworkProgramVM>();
        IEnumerable<FrameworkProgramVM> frameworkPrograms = new List<FrameworkProgramVM>();

        List<FrameworkProgramVM> selectedFrameworkProgramsList;
        FrameworkProgramVM model = new FrameworkProgramVM();
        FrameworkProgramModal frameworkProgramModal = new FrameworkProgramModal();
        FrameworkProgramVM frameworkToDelete = new FrameworkProgramVM();


        protected override async Task OnInitializedAsync()
        {
            var test = LS.GetLocalizedHtmlString("test");

            this.frameworkPrograms = (await this.FrameworkProgramService.GetAllFrameworkProgramsAsync(new FrameworkProgramVM())).OrderBy(x => x.Name).ToList();


            this.selectedFrameworkProgramsList = new List<FrameworkProgramVM>();
        }

        private async Task SelectedRow(FrameworkProgramVM model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewFPData", false);
            if (!hasPermission) { return; }
            this.model = await this.FrameworkProgramService.GetFrameworkProgramByIdAsync(model.IdFrameworkProgram);
            this.frameworkProgramModal.OpenModal(this.model);
        }

        private async Task UpdateAfterSave(FrameworkProgramVM _model)
        {
            this.frameworkPrograms = (await this.FrameworkProgramService.GetAllFrameworkProgramsAsync(new FrameworkProgramVM())).ToList();
            await this.frameworkProgramGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageFPData", false);
            if (!hasPermission) { return; }

            this.frameworkProgramModal.OpenModal(new FrameworkProgramVM());
        }

        private async Task DeleteSelected(FrameworkProgramVM model)
        {
            bool hasPermission = await CheckUserActionPermission("ManageFPData", false);
            if (!hasPermission) { return; }
            this.frameworkToDelete = model;
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                await FrameworkProgramService.RemoveFrameworkProgram(model);
                toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                await toast.sfSuccessToast.ShowAsync();
                this.frameworkPrograms = await this.FrameworkProgramService.GetAllFrameworkProgramsAsync(model);
                this.StateHasChanged();
            }
        }

        //protected async void OnFilterCall(FilterItemTemplateContext filter)
        //{
        //    ;
        //    var filterResult = filter.Record.To<FrameworkProgramVM>();
        //    var filteredResults = await this.FrameworkProgramService.GetAllFrameworkProgramsAsync(filterResult);
        //}

        private void RowSelected(RowSelectEventArgs<FrameworkProgramVM> selectArgs)
        {
            this.selectedFrameworkProgramsList.Add(selectArgs.Data);
        }

        private async Task RowDeselected(RowDeselectEventArgs<FrameworkProgramVM> selectArgs)
        {
            FrameworkProgramVM template = selectArgs.Data;

            selectedFrameworkProgramsList.Remove(template);
        }

        public class CustomComparer : IComparer<Object>
        {
            public int Compare(object XRowDataToCompare, object YRowDataToCompare)
            {
                FrameworkProgramVM XRowData = XRowDataToCompare as FrameworkProgramVM;
                FrameworkProgramVM YRowData = YRowDataToCompare as FrameworkProgramVM;
                string XFrameworkProgramNameFormatted = (string)XRowData.FrameworkProgramNameFormatted;
                string YFrameworkProgramNameFormatted = (string)YRowData.FrameworkProgramNameFormatted;
                if (String.Compare(XFrameworkProgramNameFormatted, YFrameworkProgramNameFormatted) < 0)
                {
                    return -1;
                }
                else if (String.Compare(XFrameworkProgramNameFormatted, YFrameworkProgramNameFormatted) > 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "defaultGrid_pdfexport")
            {
                int temp = frameworkProgramGrid.PageSettings.PageSize;
                frameworkProgramGrid.PageSettings.PageSize = frameworkPrograms.Count();
                await frameworkProgramGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Рамкова програма", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "VQSName", HeaderText = "СПК", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeFrameworkProgramName", HeaderText = "Вид", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FormEducationNames", HeaderText = "Форма на обучение", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "QualificationLevelName", HeaderText = "Квалификационно равнище", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SectionА", HeaderText = "Мин. брой задължителни учебни часове", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SectionB", HeaderText = "Мин. брой избираеми учебни часове", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SectionА1WithPercent", HeaderText = "Макс. % часове А1", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PracticeWithPercent", HeaderText = "Мин. % часове практическо обучение", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusValue", HeaderText = "Статус", Width = "40", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"FrameworkProgram_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.frameworkProgramGrid.ExportToPdfAsync(ExportProperties);
                frameworkProgramGrid.PageSettings.PageSize = temp;
                await frameworkProgramGrid.Refresh();
            }
            else if (args.Item.Id == "defaultGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"FrameworkProgram_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.frameworkProgramGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<FrameworkProgramVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(frameworkProgramGrid, args.Data.IdFrameworkProgram).Result.ToString();
            }
        }

    }
}
