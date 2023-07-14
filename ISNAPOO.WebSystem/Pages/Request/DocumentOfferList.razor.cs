using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request
{
    public partial class DocumentOfferList : BlazorBaseComponent
    {
        [Inject]
        public IProviderDocumentRequestService providerDocumentRequestService { get; set; }
        [Inject]
        public ICommonService CommonService { get; set; }

        private ToastMsg toast = new ToastMsg();
        private DocumentOfferModal documentOfferModal = new DocumentOfferModal();
        private SfGrid<ProviderDocumentOfferVM> sfGridDocumentOfferGrid = new SfGrid<ProviderDocumentOfferVM>();
        private IEnumerable<ProviderDocumentOfferVM> documentOfferSource = new List<ProviderDocumentOfferVM>();

        private ProviderDocumentOfferVM modelForFilterGrid { get; set; }

        private string DocumentType = string.Empty;
        private string Header = string.Empty;



        protected override async Task OnInitializedAsync()
        {
            
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.DocumentType = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.TOKEN_DOCUMENT_OFFER_TYPE_KEY).Value.ToString();

                    if (this.DocumentType == GlobalConstants.TOKEN_BORSA_DOCUMENTS_VALUE)
                    {
                        this.Header = "Борса на документи";

                        this.modelForFilterGrid = new ProviderDocumentOfferVM()
                        {
                            OfferEndDate = DateTime.Now.Date,
                        };

                        this.documentOfferSource = await this.providerDocumentRequestService.GetAllProviderDocumentOffersAsync(this.modelForFilterGrid);

                    }
                    else if (this.DocumentType == GlobalConstants.TOKEN_BORSA_DOCUMENTS_CPO_VALUE)
                    {
                        this.Header = "Публикуване на документ на борса на документи";

                        this.modelForFilterGrid = new ProviderDocumentOfferVM()
                        {
                            IdCandidateProvider = this.UserProps.IdCandidateProvider,
                        };

                        this.documentOfferSource = await this.providerDocumentRequestService.GetAllProviderDocumentOffersAsync(this.modelForFilterGrid);

                    }

                    this.StateHasChanged();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, currentContext.ListErrorMessages);
                    toast.sfErrorToast.ShowAsync();
                }

            }
        }

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageDocumentOfferData", false);
            if (!hasPermission) { return; }

            var model = new ProviderDocumentOfferVM();
            await this.documentOfferModal.OpenModal(model);
        }

        private async Task SelectedRow(ProviderDocumentOfferVM model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewDocumentOfferData", false);
            if (!hasPermission) { return; }

            var priceVM = await this.providerDocumentRequestService.GetProviderDocumentOfferByIdAsync(model);
            await this.documentOfferModal.OpenModal(priceVM);
        }

        private async Task OnApplicationSubmit(ResultContext<ProviderDocumentOfferVM> resultContext)
        {
            if (resultContext.HasMessages)
            {
                toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                await toast.sfSuccessToast.ShowAsync();
                this.documentOfferSource = await this.providerDocumentRequestService.GetAllProviderDocumentOffersAsync(this.modelForFilterGrid);
                this.sfGridDocumentOfferGrid.Refresh();
                resultContext.ListMessages.Clear();
            }
            else
            {
                toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                await toast.sfErrorToast.ShowAsync();
                resultContext.ListErrorMessages.Clear();
            }
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGridDocumentOfferGrid.PageSettings.PageSize;
                sfGridDocumentOfferGrid.PageSettings.PageSize = documentOfferSource.Count();
                await sfGridDocumentOfferGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LocationCorrespondence.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.ProviderPhoneCorrespondence", HeaderText = "Телефон", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.ProviderEmailCorrespondence", HeaderText = "E-mail", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документ", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CountOffered", HeaderText = "Брой", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OfferStartDate", HeaderText = "Начална дата", Width = "80",Format="d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OfferEndDate", HeaderText = "Начална дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });                          
                ExportColumns.Add(new GridColumn() { Field = "OfferTypeName", HeaderText = "Оферта", Width = "180", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"DocumentOffer_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGridDocumentOfferGrid.ExportToPdfAsync(ExportProperties);
                sfGridDocumentOfferGrid.PageSettings.PageSize = temp;
                await sfGridDocumentOfferGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LocationCorrespondence.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.ProviderPhoneCorrespondence", HeaderText = "Телефон", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.ProviderEmailCorrespondence", HeaderText = "E-mail", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документ", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CountOffered", HeaderText = "Брой", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OfferStartDate", HeaderText = "Начална дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OfferEndDate", HeaderText = "Начална дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OfferTypeName", HeaderText = "Оферта", Width = "180", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"DocumentOffer_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGridDocumentOfferGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ProviderDocumentOfferVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGridDocumentOfferGrid, args.Data.IdProviderDocumentOffer).Result.ToString();
            }
        }

    }
}
