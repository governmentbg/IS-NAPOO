using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class PolicySelectorModal : BlazorBaseComponent
    {
        [Inject]
        IPolicyService PolicyService { get; set; }

        [Parameter]
        public EventCallback<ResultContext<List<PolicyVM>>> CallbackAfterSelect { get; set; }

        private SfDialog sfDialog = new SfDialog();
        List<PolicyVM> policies;
        List<PolicyVM> selectedPolicy = new List<PolicyVM>();
        SfGrid<PolicyVM> refPolicyGrid = new SfGrid<PolicyVM>();
        private bool isGridButtonClicked = false;
        private List<PolicyVM> selectedPolicyselectedPolicy = new List<PolicyVM>();

        public async Task OpenModal(ApplicationRoleVM _applicationRoleVM)
        {


            var temp = await PolicyService.GetAllPolicyExceptAsync(_applicationRoleVM.RoleClaims);
            policies = temp.OrderBy(x => x.PolicyDescription).ToList();


            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task PolicyDeselectedHandler(RowDeselectEventArgs<PolicyVM> args)
        {
            this.selectedPolicy.Clear();
            this.selectedPolicy = await this.refPolicyGrid.GetSelectedRecordsAsync();
        }

        private async Task PolicySelectedHandler(RowSelectEventArgs<PolicyVM> args)
        {
            this.selectedPolicy.Clear();
            this.selectedPolicy = await this.refPolicyGrid.GetSelectedRecordsAsync();
        }

        private async Task AddNewPolicy()
        {
            this.isVisible = false;
            ResultContext<List<PolicyVM>> resultContext = new ResultContext<List<PolicyVM>>();

            resultContext.ResultContextObject = this.selectedPolicy;

            await this.CallbackAfterSelect.InvokeAsync(resultContext);
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
                ExportProperties.FileName = $"Spisak_pozvoleni_deistvia_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.refPolicyGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Spisak_pozvoleni_deistvia_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.refPolicyGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "PolicyCode", HeaderText = "Код", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "PolicyDescription", HeaderText = "Описание", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
