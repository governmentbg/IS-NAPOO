using Data.Models.Data.Request;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class NAPOOSummarizeRequestsModal : BlazorBaseComponent
    {
        private SfGrid<ProviderRequestDocumentVM> providerRequestDocumentGrid = new SfGrid<ProviderRequestDocumentVM>();
        private SfGrid<TypeOfRequestedDocumentVM> addedDocumentsGrid = new SfGrid<TypeOfRequestedDocumentVM>();
        private SfDialog sfDialog = new SfDialog();
        private NAPOOAddExtraProviderRequestDocumentsModal napooAddExtraProviderRequestDocumentsModal = new NAPOOAddExtraProviderRequestDocumentsModal();
        private ToastMsg toast = new ToastMsg();

        private List<ProviderRequestDocumentVM> providerRequestDocumentsSource = new List<ProviderRequestDocumentVM>();
        private List<ProviderRequestDocumentVM> originalProviderRequestDocuments = new List<ProviderRequestDocumentVM>();
        private List<TypeOfRequestedDocumentVM> addedDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocuments = new List<TypeOfRequestedDocumentVM>();
        private IEnumerable<KeyValueVM> kvRequestDocumetStatusSource = new List<KeyValueVM>();
        private bool isRequestSelected = false;
        private ProviderRequestDocumentVM selectedProviderRequestDocumentVM = new ProviderRequestDocumentVM();
        private NAPOORequestDocVM nAPOORequestDocVM = new NAPOORequestDocVM();
        private LocationVM locationVM = new LocationVM();
        private bool entryFromPrintingHouse = false;
        private string title = string.Empty;

        [Parameter]
        public EventCallback CallbackAfterSummarizeSubmit { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public INotificationService NotificationService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        public async Task OpenModal(List<ProviderRequestDocumentVM> providerRequestDocuments, List<ProviderRequestDocumentVM> originalProviderRequestDocuments, NAPOORequestDocVM nAPOORequestDocVM, IEnumerable<KeyValueVM> kvRequestDocumetStatusSource, bool entryFromPrintingHouse = false)
        {
            this.entryFromPrintingHouse = entryFromPrintingHouse;
            this.selectedProviderRequestDocumentVM = new ProviderRequestDocumentVM();
            this.addedDocumentsSource.Clear();
            this.isRequestSelected = false;
            this.kvRequestDocumetStatusSource = kvRequestDocumetStatusSource;
            this.nAPOORequestDocVM = nAPOORequestDocVM;
            this.providerRequestDocumentsSource = providerRequestDocuments.ToList();
            this.originalProviderRequestDocuments = originalProviderRequestDocuments;
            this.typeOfRequestedDocuments = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();

            if (this.providerRequestDocumentsSource.Any(x => !x.RequestDocumentTypes.Any()))
            {
                await this.LoadDataForDocumentTypesAsync();
            }

            this.SetTitle();

            await this.SetCreateAndModifyInfoAsync();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadDataForDocumentTypesAsync()
        {
            await this.ProviderDocumentRequestService.LoadDataForDocumentTypesAsync(this.providerRequestDocumentsSource);
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.nAPOORequestDocVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.nAPOORequestDocVM.IdModifyUser);
            this.nAPOORequestDocVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.nAPOORequestDocVM.IdCreateUser);
        }

        private void SetTitle()
        {
            var requestNumberInfo = this.nAPOORequestDocVM.NAPOORequestNumber.HasValue ? $"№{this.nAPOORequestDocVM.NAPOORequestNumber}/" : string.Empty;
            var requestDateInfo = this.nAPOORequestDocVM.RequestDate.HasValue ? $"{this.nAPOORequestDocVM.RequestDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г., " : string.Empty;
            var statusInfo = this.nAPOORequestDocVM.RequestDate.HasValue ? this.nAPOORequestDocVM.IsSent ? "Изпратена към печатница" : string.Empty : "В процес на обобщаване";
            var statusValue = statusInfo == "В процес на обобщаване" ? ", Статус" : "Статус";
            this.title = $"Данни за обобщена заявка <span style=\"color: #ffffff;\">{requestNumberInfo}{requestDateInfo}</span>{statusValue} <span style=\"color: #ffffff;\">{statusInfo}</span>";
        }

        private async Task RequestSelected(RowSelectEventArgs<ProviderRequestDocumentVM> args)
        {
            this.isRequestSelected = true;
            await this.SetDataForSelectedProviderDocumentRequest(args);
        }

        private void RequestDeselected(RowDeselectEventArgs<ProviderRequestDocumentVM> args)
        {
            this.isRequestSelected = false;
            this.selectedProviderRequestDocumentVM = new ProviderRequestDocumentVM();
        }

        private async Task SetDataForSelectedProviderDocumentRequest(RowSelectEventArgs<ProviderRequestDocumentVM> args)
        {
            this.SpinnerShow();

            this.addedDocumentsSource.Clear();
            this.selectedProviderRequestDocumentVM = this.providerRequestDocumentsSource.FirstOrDefault(x => x.IdProviderRequestDocument == args.Data.IdProviderRequestDocument);
            this.locationVM = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.selectedProviderRequestDocumentVM.IdLocationCorrespondence ?? default);
            this.selectedProviderRequestDocumentVM.LocationName = this.locationVM.DisplayJoinedNames;

            foreach (var docType in this.selectedProviderRequestDocumentVM.RequestDocumentTypes)
            {
                var addedDocType = this.typeOfRequestedDocuments.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docType.IdTypeOfRequestedDocument);
                addedDocType.Quantity = docType.DocumentCount;
                this.addedDocumentsSource.Add(addedDocType);
            }

            this.selectedProviderRequestDocumentVM.RequestStatus = this.kvRequestDocumetStatusSource.FirstOrDefault(x => x.IdKeyValue == this.selectedProviderRequestDocumentVM.IdStatus)?.Name;

            this.addedDocumentsSource = this.addedDocumentsSource.OrderBy(x => x.IdTypeOfRequestedDocument).ToList();
            this.addedDocumentsGrid.Refresh();

            this.SpinnerHide();
        }

        private async Task AddExtraRequestDocuments()
        {
            bool hasPermission = await CheckUserActionPermission("ManageSummarizedRequestDocumentData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.napooAddExtraProviderRequestDocumentsModal.OpenModal(this.providerRequestDocumentsSource, this.originalProviderRequestDocuments);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void UpdateAfterModalSubmit(List<ProviderRequestDocumentVM> providerRequestDocuments)
        {
            this.providerRequestDocumentsSource.AddRange(providerRequestDocuments);
            this.providerRequestDocumentGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task SaveRequestDocumentsForSummary()
        {
            bool hasPermission = await CheckUserActionPermission("ManageSummarizedRequestDocumentData", false);
            if (!hasPermission) { return; }
            this.SpinnerShow();
            if (this.providerRequestDocumentsSource.Any())
            {
                this.nAPOORequestDocVM.RequestYear = this.providerRequestDocumentsSource.FirstOrDefault().CurrentYear;
            }

            this.nAPOORequestDocVM.IsSent = false;
            this.nAPOORequestDocVM.UploadedFileName = string.Empty;
            this.nAPOORequestDocVM.ProviderRequestDocuments = this.providerRequestDocumentsSource;

            var inputContext = new ResultContext<NAPOORequestDocVM>();
            inputContext.ResultContextObject = this.nAPOORequestDocVM;

            var resultContext = await this.ProviderDocumentRequestService.CreateNAPOORequestDocumentAsync(inputContext);
            this.nAPOORequestDocVM.IdNAPOORequestDoc = resultContext.ResultContextObject.IdNAPOORequestDoc;

            if (resultContext.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
            }
            else
            {
                await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                foreach (var providerRequest in this.providerRequestDocumentsSource)
                {
                    providerRequest.RequestStatus = this.kvRequestDocumetStatusSource.FirstOrDefault(x => x.IdKeyValue == providerRequest.IdStatus)?.Name;
                }

                this.addedDocumentsGrid.Refresh();
                await this.CallbackAfterSummarizeSubmit.InvokeAsync();

                await this.SetCreateAndModifyInfoAsync();
            }
            this.SpinnerHide();
        }

        private async Task GeneratePrintingTemplate(bool printPDF = false)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var result = await this.ProviderDocumentRequestService.GeneratePrintingTemplateAsync(this.typeOfRequestedDocuments, this.nAPOORequestDocVM, printPDF);

                if (!printPDF)
                {
                    await FileUtils.SaveAs(this.JsRuntime, "Obobshteni_zaiavki_template.xlsx", result.ToArray());
                }
                else
                {
                    await FileUtils.SaveAs(this.JsRuntime, "Obobshteni_zaiavki_template.pdf", result.ToArray());
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task GenerateFileForMONPrinting()
        {
            bool hasPermission = await CheckUserActionPermission("ManageSummarizedRequestDocumentData", false);
            if (!hasPermission) { return; }

            string msg = "Сигурни ли сте, че искате да изпратите заявката към печатницата? След тази операция, няма да може да извършвате промени в заявката.";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    this.nAPOORequestDocVM.ProviderRequestDocuments = this.providerRequestDocumentsSource;
                    var inputContext = new ResultContext<NAPOORequestDocVM>();
                    inputContext.ResultContextObject = this.nAPOORequestDocVM;

                    var result = await this.ProviderDocumentRequestService.GenerateFileForMONPrinting(inputContext);
                    this.nAPOORequestDocVM = result.ResultContextObject;

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        foreach (var providerRequest in this.providerRequestDocumentsSource)
                        {
                            providerRequest.RequestStatus = this.kvRequestDocumetStatusSource.FirstOrDefault(x => x.IdKeyValue == providerRequest.IdStatus).Name;
                        }

                        this.addedDocumentsGrid.Refresh();
                        await this.CallbackAfterSummarizeSubmit.InvokeAsync();
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task SendNotificationsClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageSummarizedRequestDocumentData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var result = string.Empty;
                var listPersonIds = new List<int>();
                foreach (var providerReqDoc in this.providerRequestDocumentsSource)
                {
                    var listIds = await this.NotificationService.GetAllPersonIdsByCandidateProviderIdAsync(providerReqDoc.IdCandidateProvider);
                    var about = $"Заявка за документация №{providerReqDoc.RequestNumber}/{providerReqDoc.RequestDate.Value.ToString("dd.MM.yyyy")}г.";
                    var message = $"Вашата заявка, регистрирана с №{providerReqDoc.RequestNumber}/{providerReqDoc.RequestDate.Value.ToString("dd.MM.yyyy")}г., е одобрена от Националната агенция за професионално образование и обучение и е обобщена в заявка №{this.nAPOORequestDocVM.NAPOORequestNumber}/{this.nAPOORequestDocVM.RequestDate.Value.ToString("dd.MM.yyyy")}г., представена в Печатница „Образование и наука“.";

                    NotificationVM notificationVM = new NotificationVM()
                    {
                        About = about,
                        NotificationText = message,
                    };

                    result = await this.NotificationService.CreateNotificationForListOfPersonIdsAsync(notificationVM, listIds.ToList());
                    if (!result.Contains("Успешно"))
                    {
                        await this.ShowErrorAsync("Грешка при запис в базата данни!");
                        return;
                    }
                    else
                    {
                        await this.ProviderDocumentRequestService.UpdateNotificationSentStatusForNAPOORequestDocAsync(this.nAPOORequestDocVM);
                        await this.CallbackAfterSummarizeSubmit.InvokeAsync();
                    }
                }

                await this.ShowSuccessAsync(result);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task PrintRequestDocument()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                if (this.selectedProviderRequestDocumentVM.IdProviderRequestDocument == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете заявка!");
                    return;
                }

                var isDocumentUploaded = !string.IsNullOrEmpty(this.selectedProviderRequestDocumentVM.UploadedFileName);

                if (isDocumentUploaded)
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ProviderRequestDocument>(this.selectedProviderRequestDocumentVM.IdProviderRequestDocument);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ProviderRequestDocument>(this.selectedProviderRequestDocumentVM.IdProviderRequestDocument);

                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.selectedProviderRequestDocumentVM.FileName, documentStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        await this.GenerateRequestReportAsync();
                    }
                }
                else
                {
                    await this.GenerateRequestReportAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task GenerateRequestReportAsync()
        {
            var typeOfRequestedDocumentSource = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();
            var result = await this.ProviderDocumentRequestService.PrintRequestDocumentAsync(this.selectedProviderRequestDocumentVM, typeOfRequestedDocumentSource, this.selectedProviderRequestDocumentVM.CandidateProvider);
            await FileUtils.SaveAs(this.JsRuntime, "Zaqvka_dokumentaciq_CPO.pdf", result.ToArray());

            var resultFromUpload = await this.UploadFileService.UploadFileAsync<ProviderRequestDocument>(result, "Zaqvka_dokumentaciq_CPO.pdf", "ProviderRequestDocument", this.selectedProviderRequestDocumentVM.IdProviderRequestDocument);
            if (!string.IsNullOrEmpty(resultFromUpload))
            {
                this.selectedProviderRequestDocumentVM.UploadedFileName = resultFromUpload;
            }

            await this.ProviderDocumentRequestService.UpdateProviderRequestDocumentUploadedFileNameAsync(this.selectedProviderRequestDocumentVM.IdProviderRequestDocument, this.selectedProviderRequestDocumentVM.UploadedFileName);
        }
    }
}
