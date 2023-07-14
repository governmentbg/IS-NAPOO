using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class RolesList : BlazorBaseComponent
    {
        List<ApplicationRoleVM> roles;
        List<ApplicationRoleVM> originalRoles;

        SfGrid<ApplicationRoleVM> refGrid;

        private ToastMsg toast = new ToastMsg();
        private RolesListFilterModal rolesListFilterModal = new RolesListFilterModal();
        private RoleModal roleModal = new RoleModal();
        int row = 1;
        protected override async Task OnInitializedAsync()
        {
            var rolesSource = await ApplicationUserService.GetAllApplicationRoleAsync(new ApplicationRoleVM());
            this.roles = rolesSource.OrderBy(v => v.Name).ToList();
            this.originalRoles = this.roles;
            
        } 

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageRolesData", false);
            if (!hasPermission) { return; }

            await this.roleModal.OpenModal(new ApplicationRoleVM());
        }

        private async Task SelectedRow(ApplicationRoleVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewRolesData", false);
            if (!hasPermission) { return; }

            await this.roleModal.OpenModal(_model);
        }
        private void OnFilterModalSubmit(List<ApplicationRoleVM> roles)
        {


            this.roles = roles.ToList();


            this.refGrid.Refresh();
            StateHasChanged();
        }

        private async Task OnApplicationSubmit(ResultContext<ApplicationRoleVM> resultContext)
        {
            var msg = resultContext.ListMessages.FirstOrDefault();

            if (resultContext.ListMessages.Count() == 0 )
            {
                this.toast.sfSuccessToast.Content = "Записът е успешен!";
                await this.toast.sfSuccessToast.ShowAsync();
                var rolesSource = await ApplicationUserService.GetAllApplicationRoleAsync(new ApplicationRoleVM());
                this.roles = rolesSource.OrderBy(v => v.Name).ToList();
            }
            else
            {
                var errorMsg = resultContext.ListErrorMessages.FirstOrDefault();
                this.toast.sfErrorToast.Content = msg + Environment.NewLine + errorMsg;
                await this.toast.sfErrorToast.ShowAsync();
            }
        }
        public async Task<string> GetRowNumber(string key)
        {
            var page = refGrid.PageSettings.CurrentPage - 1;
            var pageLength = refGrid.PageSettings.PageSize;
            var index = await refGrid.GetRowIndexByPrimaryKeyAsync(key);
            var num = page * pageLength + index;

            return $"{num + 1}.";
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "roleGrid_pdfexport")
            {
                int temp = refGrid.PageSettings.PageSize;
                refGrid.PageSettings.PageSize = roles.Count();
                await refGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Код", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RoleName", HeaderText = "Роля на потребител", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"Roles_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.refGrid.ExportToPdfAsync(ExportProperties);
                refGrid.PageSettings.PageSize = temp;
                await refGrid.Refresh();
            }
            else if (args.Item.Id == "roleGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Roles_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.refGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ApplicationRoleVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(args.Data.Id).Result.ToString();
            }
        }

        private async Task OpenFilterBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var roles = (await ApplicationUserService.GetAllApplicationRoleAsync()).OrderBy(v => v.Name).ToList();
                this.originalRoles = roles.ToList();
                
                this.rolesListFilterModal.OpenModal(this.originalRoles);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
