using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Common.Notifications;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate.NAPOO
{
    public partial class AdminNotificationList : BlazorBaseComponent
    {
        private SfGrid<NotificationVM> currentGrid = new SfGrid<NotificationVM>();
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private NotificationModal editModal = new NotificationModal();

        private int? idCandidateProvider = null;
        private IEnumerable<NotificationVM> dataSource = new List<NotificationVM>();
        private IEnumerable<NotificationVM> originalDataSource = new List<NotificationVM>();
        private List<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private NotificationVM model = new NotificationVM();
        private int idPerson = 0;
        private bool isFilteredByCandidateProvider = false;
        private string providerType = string.Empty;

        [Inject]
        public INotificationService NotificationService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                await this.HandleTokenData();

                await this.currentGrid.Refresh();
                this.StateHasChanged();

                this.SpinnerHide();
            }
        }

        private async Task HandleTokenData()
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    var userID = await this.DataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
                    var user = await this.ApplicationUserService.GetApplicationUsersById(userID.SettingValue);

                    var entryFrom = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();
                    if (entryFrom == GlobalConstants.TOKEN_ADMIN_NOTIFICATIONLIST_CPO)
                    {
                        this.providerType = "ЦПО";
                    }
                    else if (entryFrom == GlobalConstants.TOKEN_ADMIN_NOTIFICATIONLIST_CIPO)
                    {
                        this.providerType = "ЦИПО";
                    }

                    this.dataSource = await this.NotificationService.GetAllNotificationsForProviderTypeByPersonFromAsync(user.IdPerson, entryFrom == GlobalConstants.TOKEN_ADMIN_NOTIFICATIONLIST_CPO);
                    this.originalDataSource = this.dataSource.ToList();

                    var kvCPO = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
                    this.candidateProvidersSource = await this.CandidateProviderService.GetCandidateProvidersAsync(this.dataSource.Where(x => x.IdPersonTo.HasValue).Select(x => x.IdPersonTo!.Value).ToList());
                    foreach (var cand in this.candidateProvidersSource)
                    {
                        if (cand.IdTypeLicense == kvCPO.IdKeyValue)
                        {
                            cand.MixCPOandCIPONameOwner = cand.CPONameOwnerGrid;
                        }
                        else
                        {
                            cand.MixCPOandCIPONameOwner = cand.CIPONameOwnerGrid;
                        }
                    }
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private async Task SelectedRow(NotificationVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewNotificationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.editModal.OpenModal(_model);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task FilterNotificationsBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.idCandidateProvider.HasValue)
                {
                    var candidateProviderPersons = await this.CandidateProviderService.GetAllActiveCandidateProviderPersonsByIdCandidateProviderAsync(this.idCandidateProvider.Value);
                    var personIdsList = candidateProviderPersons.Select(x => x.IdPerson).ToList();

                    this.dataSource = this.dataSource.Where(x => x.IdPersonTo.HasValue && personIdsList.Contains(x.IdPersonTo!.Value)).ToList();

                    this.isFilteredByCandidateProvider = true;

                    await this.currentGrid.Refresh();
                    this.StateHasChanged();
                }
                else
                {
                    await this.ShowErrorAsync("Моля, изберете център преди да филтрирате!");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task ClearFilterBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.dataSource = this.originalDataSource.ToList();

                this.isFilteredByCandidateProvider = false;

                this.idCandidateProvider = null;

                await this.currentGrid.Refresh();
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "ProviderJoinedInformation", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.candidateProvidersSource, query);
        }

        private void CellInfoHandler(QueryCellInfoEventArgs<NotificationVM> args)
        {

            if (args.Data.StatusNotificationName != null && args.Data.StatusNotificationName.ToLower() == "непрочетено")
            {
                args.Cell.AddClass(new string[] { "text-danger" });
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = currentGrid.PageSettings.PageSize;
                currentGrid.PageSettings.PageSize = dataSource.Count();
                await currentGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "About", HeaderText = "Относно", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "NotificationText", HeaderText = "Коментар", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SendDateAsStr", HeaderText = "Дата на изпращане", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReviewDateAsStr", HeaderText = "Дата на прочитане", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonFrom.FullName", HeaderText = "Изпратено от", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonTo.FullName", HeaderText = "Изпратено до", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusNotificationName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"Notification_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.currentGrid.ExportToPdfAsync(ExportProperties);
                currentGrid.PageSettings.PageSize = temp;
                await currentGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Notification_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.currentGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<NotificationVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(currentGrid, args.Data.IdNotification).Result.ToString();
            }
        }
    }
}

