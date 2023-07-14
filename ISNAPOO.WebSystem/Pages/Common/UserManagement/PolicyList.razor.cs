using System;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class PolicyList : BlazorBaseComponent
    {
        IEnumerable<PolicyVM> policies;

        [Inject]
        public IPolicyService PolicyService { get; set; }

        SfGrid<PolicyVM> refGrid;

        private PolicyModal refModal = new PolicyModal();

        protected override async Task OnInitializedAsync()
        {
            policies = await PolicyService.GetAllPolicyAsync(new PolicyVM());
        }

        private async Task SelectRow(PolicyVM model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewPolicyData", false);
            if (!hasPermission) { return; }

            policies = await PolicyService.GetAllPolicyAsync(new PolicyVM());

            await this.refModal.openModal(await PolicyService.getById(model.idPolicy));
        }

        private async Task refreshAfterModalSubmit()
        {
            policies = await PolicyService.GetAllPolicyAsync(new PolicyVM());

            await refGrid.RefreshColumnsAsync();
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "roleGrid_pdfexport")
            {
                int temp = refGrid.PageSettings.PageSize;
                refGrid.PageSettings.PageSize = policies.Count();
                await refGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "PolicyCode", HeaderText = "Код", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PolicyDescription", HeaderText = "Описание", Width = "80", TextAlign = TextAlign.Left });
                
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
                ExportProperties.FileName = $"Policies_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.refGrid.ExportToPdfAsync(ExportProperties);
                refGrid.PageSettings.PageSize = temp;
                await refGrid.Refresh();
            }
            else if (args.Item.Id == "roleGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Policies_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.refGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<PolicyVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(refGrid, args.Data.idPolicy).Result.ToString();
            }
        }
    }
}

