using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class FollowUpControlList : BlazorBaseComponent
    {
        private FollowUpControlModal followUpControlModal;
        private FollowUpControlListFilterModal followUpControlListFilterModal = new FollowUpControlListFilterModal();
        private SfGrid<FollowUpControlVM> sfGrid;
        private IEnumerable<FollowUpControlVM> controlsSource = new List<FollowUpControlVM>();
        private string Header = string.Empty;
        private string CPOorCIPO = string.Empty;
        private string LicensingType = string.Empty;
        private bool IsShowFilter = false;
        protected override async Task OnInitializedAsync()
        {
            this.LicensingType = string.Empty;
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = Token;
            tokenContext = BaseHelper.GetDecodeToken(tokenContext);


            this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "FollowUpControlType").Value.ToString();
            if (this.LicensingType == "LicensingCPO")
            {
                
                Header = "Последващ контрол ЦПО";
                CPOorCIPO = "ЦПО";
                
            }
            else
            {
                Header = "Последващ контрол ЦИПО";
                CPOorCIPO = "ЦИПО";
            }
            this.controlsSource = await this.controlService.GetAllControlsAsync(this.LicensingType);
            await this.sfGrid.Refresh();
            this.IsShowFilter = true;
            this.StateHasChanged();
        }
        public void OpenNewModal()
        {
            this.followUpControlModal.OpenModal(new FollowUpControlVM());
        }

        private async Task UpdateAfterSave()
        {
            this.controlsSource = (await this.controlService.GetAllControlsAsync(this.LicensingType)).ToList();
            this.StateHasChanged();
        }

        private async Task SelectedRow(FollowUpControlVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewFollowUpControlData", false);
            if (!hasPermission) { return; }

            this.followUpControlModal.OpenModal(_model);
        }

        public async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = controlsSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                PdfTheme Theme = new PdfTheme();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "180", TextAlign = TextAlign.Left });

                if (CPOorCIPO == "ЦПО")
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                else
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "80", TextAlign = TextAlign.Left });

                ExportColumns.Add(new GridColumn() { Field = "FollowUpControlTypeName", HeaderText = "Вид на последващия контрол", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlTypeName", HeaderText = "Вид на проверката", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlStartDate", HeaderText = "Срок за проверката от", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "ControlEndDate", HeaderText = "Срок за проверката до", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "TermImplRecommendation", HeaderText = "Срок за изпълнение на препоръки", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left, });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String },/*you fonts famiy in form of base64string*/
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String },/*you fonts famiy in form of base64string*/
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = "Posledvasht_kontrol.pdf";
                await this.sfGrid.PdfExport(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "150", TextAlign = TextAlign.Left }); 

                if (CPOorCIPO == "ЦПО")
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                else
                    ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "80", TextAlign = TextAlign.Left });

                ExportColumns.Add(new GridColumn() { Field = "FollowUpControlTypeName", HeaderText = "Вид на последващия контрол", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlTypeName", HeaderText = "Вид на проверката", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ControlStartDate", HeaderText = "Срок за проверката от", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "ControlEndDate", HeaderText = "Срок за проверката до", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "TermImplRecommendation", HeaderText = "Срок за изпълнение на препоръки", Format = "d", Width = "80", TextAlign = TextAlign.Left, });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left, });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = "Posledvasht_kontrol.xlsx";
                await this.sfGrid.ExcelExport(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<FollowUpControlVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdFollowUpControl).Result.ToString();
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

                var controlsSourceFilter = await this.controlService.GetAllControlsAsync(this.LicensingType);
                this.followUpControlListFilterModal.OpenModal(controlsSourceFilter.ToList());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private void OnFilterModalSubmit(List<FollowUpControlVM> followUpControls)
        {
            if (followUpControls.Count == 0)
            {
                this.controlsSource = new List<FollowUpControlVM>();
            }
            else
            {
                this.controlsSource = followUpControls.ToList();
            }
            this.StateHasChanged();
        }
    }
}
