using System.Linq;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.CPO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO.Parser.Biff_Records;
using Data.Models.Data.ProviderData;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{

    public partial class UsersList : BlazorBaseComponent
    {
        private ChangeProviderUserToProviderModal changeProviderToUserModal = new ChangeProviderUserToProviderModal();

        IEnumerable<ApplicationUserVM> users;
        IEnumerable<ApplicationRoleVM> roleSource;
        SfGrid<ApplicationUserVM> refGrid;
        ToastMsg toast;        
        SfMultiSelect<List<string>, ApplicationRoleVM> sfMultiSelect = new SfMultiSelect<List<string>, ApplicationRoleVM>();
        private List<ApplicationUserVM> selectedRows = new List<ApplicationUserVM>();
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private List<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private List<int> ids = new List<int>();
        private UserModal userModal = new UserModal();
        public bool IsFilterVisible { get; set; } = false;
        public bool IsSearchConfirmed { get; set; } = false;
        FilterModel model = new FilterModel();

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.users = await ApplicationUserService.GetAllApplicationUserAsync(new ApplicationUserVM());
            this.roleSource = await ApplicationUserService.GetAllApplicationRoleAsync(new ApplicationRoleVM());
            this.roleSource = roleSource.OrderBy(r => r.RoleName).ToList();
        }

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageUserData", false);
            if (!hasPermission) { return; }

            await this.userModal.OpenModal(new ApplicationUserVM());

        }

        private async Task SelectedRow(ApplicationUserVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewUserData", false);
            if (!hasPermission) { return; }

            await this.userModal.OpenModal(_model);
        }

        private async Task RowDeselectedHandler(RowDeselectEventArgs<ApplicationUserVM> args)
        {
            this.selectedRows.Clear();
            this.selectedRows = await this.refGrid.GetSelectedRecordsAsync();
        }

        private async Task RowSelectedHandler(RowSelectEventArgs<ApplicationUserVM> args)
        {
            this.selectedRows.Clear();
            this.selectedRows = await this.refGrid.GetSelectedRecordsAsync();
        }

        public async Task ChangePasswords()
        {
            if (!this.selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред/редове!");
                return;
            }

            if (loading) return;

            try
            {
                loading = true;
                this.SpinnerShow();

                foreach (var row in selectedRows)
                {
                 await ApplicationUserService.SendPassword(row.Id);
                }
               
                this.SpinnerHide();

                if (selectedRows.Count == 1)
                {
                    await this.ShowSuccessAsync("Паролата е нулирана успешно и е изпратен автоматичен e-mail с новата парола до потребителя!");
                }
                else
                {
                    await this.ShowSuccessAsync("Паролите са нулирани успешно и са изпратени автоматични имейли с новите пароли до потребителите!");
                }
            }
            finally
            {
                loading = false;
            }
        }
        private async void OnKeyPress(KeyboardEventArgs args)
        {
            if (args.Key.ToString() == "Enter")
            {
                this.IsSearchConfirmed = true; 
                await FilterGrid();
            }
        }
        private async Task OnUserSubmit(ResultContext<ApplicationUserVM> resultContext)
        {
            this.users = await ApplicationUserService.GetAllApplicationUserAsync(new ApplicationUserVM());
            this.roleSource = await ApplicationUserService.GetAllApplicationRoleAsync(new ApplicationRoleVM());
            this.roleSource = roleSource.OrderBy(r => r.RoleName).ToList();
            this.IsSearchConfirmed = true;
            await FilterGrid();
            this.IsFilterVisible = false;
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "userGrid_pdfexport")
            {
                int temp = refGrid.PageSettings.PageSize;
                refGrid.PageSettings.PageSize = users.Count();
                await refGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "UserName", HeaderText = "Потребител", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FirstName", HeaderText = "Име", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FamilyName", HeaderText = "Фамилия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Phone", HeaderText = "Телефон", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Email", HeaderText = "mail", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RolesInfo", HeaderText = "Роли", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "UserStatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"Users_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.refGrid.ExportToPdfAsync(ExportProperties);
                refGrid.PageSettings.PageSize = temp;
                await refGrid.Refresh();
            }
            else if (args.Item.Id == "userGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Users_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.refGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ApplicationUserVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(args.Data.Id).Result.ToString();
            }
        }
        private async Task FilterGrid()
        {
            bool hasPermission = await CheckUserActionPermission("ViewNomenclaturesData", false);
            if (!hasPermission) { return; }
            this.IsFilterVisible = !this.IsFilterVisible;
            model.Indent = model.Indent.Trim();
            List<ApplicationUserVM> filteredUsers = new List<ApplicationUserVM>();
            List<ApplicationUserVM> tmpFilterUsersByDate = new List<ApplicationUserVM>();       
            var kvCPO = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
            var kvCIPO = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");
            
            var allUsers = await ApplicationUserService.GetAllApplicationUserAsync(new ApplicationUserVM());
            allUsers = allUsers.Where(u => u.Person != null).ToList();

            var idPerson = allUsers.Select(x => x.IdPerson).ToList();
           
            this.candidateProvidersSource = await candidateProviderService.GetCandidateProvidersAsync(idPerson);
            foreach (var cand in this.candidateProvidersSource)
            {
                if (cand.IdTypeLicense == kvCPO.IdKeyValue)
                {
                    cand.MixCPOandCIPONameOwner = cand.CPONameOwnerGrid;
                }
                else if (cand.IdTypeLicense == kvCIPO.IdKeyValue)
                {
                    cand.MixCPOandCIPONameOwner = cand.CIPONameOwnerGrid;
                }
            }

            if (IsSearchConfirmed)
            {

                if (model.IdCandidateProvider == null && model.UserCreatedDateFrom == null && model.UserCreatedDateTo == null && string.IsNullOrEmpty(model.Indent) && (model.RoleIds == null || model.RoleIds.Count == 0))
                {
                    this.users = await ApplicationUserService.GetAllApplicationUserAsync(new ApplicationUserVM());
                    return;
                }

                if (model.IdCandidateProvider != null)
                {
                    var candidateProviderPersons = await candidateProviderService.GetCandidateProviderPerson(model.IdCandidateProvider);
                    var idPersons = candidateProviderPersons.Select(x => x.IdPerson).ToList();
                    filteredUsers = allUsers.Where(x => idPersons.Contains(x.IdPerson)).ToList();

                }
                if (filteredUsers.Count > 0 || (filteredUsers.Count == 0 && model.IdCandidateProvider != null))
                {
                    allUsers = filteredUsers;
                }

                if (model.UserCreatedDateFrom != null && model.UserCreatedDateTo != null)
                {
                    filteredUsers = allUsers.Where(x => x.CreationDate.Date >= model.UserCreatedDateFrom && x.CreationDate.Date <= model.UserCreatedDateTo).ToList();
                }
                else if (model.UserCreatedDateFrom != null && model.UserCreatedDateTo == null)
                {
                    filteredUsers = allUsers.Where(x => x.CreationDate.Date >= model.UserCreatedDateFrom).ToList();
                }
                else if (model.UserCreatedDateFrom == null && model.UserCreatedDateTo != null)
                {
                    filteredUsers = allUsers.Where(x => x.CreationDate.Date <= model.UserCreatedDateTo).ToList();
                }
                

                if (filteredUsers.Any() || (!filteredUsers.Any() && (model.UserCreatedDateFrom != null || model.UserCreatedDateTo != null)))
                {
                    allUsers = filteredUsers;
                }

                if (!string.IsNullOrEmpty(model.Indent) && model.RoleIds != null && model.RoleIds.Count != 0)
                {
                    filteredUsers = allUsers.Where(x => model.RoleIds.All(y => x.Roles.Any(c => c.Id == y)) && (((x.Person.Indent != null) ? x.Person.Indent == model.Indent : false) || (x.Person.Indent != null ? x.Person.Indent.Contains(this.model.Indent.ToLower()) : false))).ToList();
                }
                else if (!string.IsNullOrEmpty(model.Indent) && (model.RoleIds == null || model.RoleIds.Count == 0))
                {
                    filteredUsers = allUsers.Where(u => ((u.Person.Indent != null) ? u.Person.Indent == model.Indent : false) || (u.Person.Indent != null ? u.Person.Indent.Contains(this.model.Indent.ToLower()) : false)).ToList();
                }
                else if (string.IsNullOrEmpty(model.Indent) && (model.RoleIds != null && model.RoleIds.Count != 0))
                {
                    filteredUsers = allUsers.Where(x => model.RoleIds.All(y => x.Roles.Any(c => c.Id == y))).ToList();
                }

              //  if (filteredUsers.Count() == 0)
               // {
               //     toast.sfErrorToast.Content = "Няма намерен потребител, който да отговаря на зададените критерии за търсене!";
              //      await toast.sfErrorToast.ShowAsync();
              //  }
                users = filteredUsers;
            }
            IsSearchConfirmed = false;
            this.StateHasChanged();
        }
        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;   

            var query = new Query().Where(new WhereFilter() { Field = "MixCPOandCIPONameOwner", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.candidateProvidersSource, query);
        }
        private async Task ClearFilter()
        {
            this.users = await ApplicationUserService.GetAllApplicationUserAsync(new ApplicationUserVM());
            await sfMultiSelect.ClearAsync();
            model.IdCandidateProvider = null;
            model.Indent = "";
            model.UserCreatedDateFrom = null;
            model.UserCreatedDateTo = null;
        }

        private async Task OpenChangeProviderToUserBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.changeProviderToUserModal.OpenModal();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task<string> GetRowNumber(string key)
        {
            var page = refGrid.PageSettings.CurrentPage - 1;
            var pageLength = refGrid.PageSettings.PageSize;
            var index = await refGrid.GetRowIndexByPrimaryKeyAsync(key);
            var num = page * pageLength + index;

            return $"{num + 1}.";
        }
    }
    public class FilterModel
    {
        public FilterModel()
        {
            Indent = "";
            RoleIds = new List<string>();
        }
        public int? IdCandidateProvider { get; set; }
        public string Indent { get; set; }
        public List<string> RoleIds { get; set; }
        public DateTime? UserCreatedDateFrom { get; set; }
        public DateTime? UserCreatedDateTo { get; set; }
    }
}
