using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPODocumentRequestList : BlazorBaseComponent
    {
        private SfGrid<ProviderRequestDocumentVM> sfGrid = new SfGrid<ProviderRequestDocumentVM>();
        private CPODocumentRequestModal documentRequestModal = new CPODocumentRequestModal();

        private IEnumerable<ProviderRequestDocumentVM> providerRequestSource = new List<ProviderRequestDocumentVM>();
        private IEnumerable<KeyValueVM> kvRequestDocumetStatusSource = new List<KeyValueVM>();
        private CandidateProviderVM providerVM = new CandidateProviderVM();
        private ProviderRequestDocumentVM requestToDelete = new ProviderRequestDocumentVM();
        private int idCreatedKv = 0;

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IPersonService PersonService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.kvRequestDocumetStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestDocumetStatus");
                this.idCreatedKv = this.kvRequestDocumetStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Created").IdKeyValue;
                this.providerVM = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);

                await this.SetProviderRequestDataAsync();

                await this.sfGrid.Refresh();
                this.StateHasChanged();
            }

        }

        private async Task SetProviderRequestDataAsync()
        {
            this.SpinnerShow();

            this.providerRequestSource = await this.ProviderDocumentRequestService.GetAllDocumentRequestsByCandidateProviderIdAsync(this.providerVM.IdCandidate_Provider);
            foreach (var providerRequest in this.providerRequestSource)
            {
                providerRequest.CandidateProvider = this.providerVM;
                providerRequest.RequestStatus = this.kvRequestDocumetStatusSource.FirstOrDefault(x => x.IdKeyValue == providerRequest.IdStatus)?.Name;
            }

            this.SpinnerHide();
        }

        private async Task EditRequest(ProviderRequestDocumentVM providerRequestDocumentVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewCPODocumentRequestData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var status = providerRequestDocumentVM.RequestStatus;
                var providerRequestDocument = await this.ProviderDocumentRequestService.GetProviderRequestDocumentByIdAsync(providerRequestDocumentVM);
                providerRequestDocument.RequestStatus = status;
                providerRequestDocumentVM.IdLocationCorrespondence = this.providerVM.IdLocationCorrespondence;
                providerRequestDocumentVM.Address = this.providerVM.ProviderAddressCorrespondence;
                providerRequestDocumentVM.Telephone = this.providerVM.ProviderPhoneCorrespondence;

                await this.documentRequestModal.OpenModal(providerRequestDocument, this.providerVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentRequestData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var person = await this.PersonService.GetPersonByIdAsync(this.UserProps.IdPerson);
                ProviderRequestDocumentVM providerRequestDocumentVM = new ProviderRequestDocumentVM()
                {
                    IdCandidateProvider = this.providerVM.IdCandidate_Provider,
                    Address = this.providerVM.ProviderAddressCorrespondence,
                    IdLocationCorrespondence = this.providerVM.IdLocationCorrespondence,
                    Telephone = this.providerVM.ProviderPhoneCorrespondence,
                    Name = $"{person.FirstName} {person.FamilyName}",
                    Position = person.Position,
                    IsSent = false,
                    UploadedFileName = string.Empty
                };

                await this.documentRequestModal.OpenModal(providerRequestDocumentVM, this.providerVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnAfterRequestModalSave()
        {
            await this.SetProviderRequestDataAsync();

            await this.sfGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task DeleteRequest(ProviderRequestDocumentVM providerRequestDocumentVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentRequestData", false);
            if (!hasPermission) { return; }

            this.requestToDelete = providerRequestDocumentVM;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
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

                    var result = await this.ProviderDocumentRequestService.DeleteProviderRequestDocumentByIdAsync(this.requestToDelete);

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.SetProviderRequestDataAsync();
                        await this.sfGrid.Refresh();
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

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = providerRequestSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "RequestNumber", HeaderText = "Заявка №", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestDate", HeaderText = "Дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Отговорно лице", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Position", HeaderText = "Длъжност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CurrentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestStatus", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"CPO_DocumentRequest_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { Field = "RequestNumber", HeaderText = "Заявка №", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestDate", HeaderText = "Дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Отговорно лице", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Position", HeaderText = "Длъжност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CurrentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestStatus", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"CPO_DocumentRequest_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ProviderRequestDocumentVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdProviderRequestDocument).Result.ToString();
            }
        }
    }
}
