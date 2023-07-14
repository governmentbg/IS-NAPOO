using Data.Models.Data.Request;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPORequestDocumentManagementModal : BlazorBaseComponent
    {
        private SfDialog cpoRequestManagementModal = new SfDialog();
        private ToastMsg toast = new ToastMsg();
        private SfGrid<DocumentSerialNumberVM> addedFabricNumbersGrid = new SfGrid<DocumentSerialNumberVM>();
        private CPOConsecutiveNumbersModal cpoConsecutiveNumbersModal = new CPOConsecutiveNumbersModal();
        private UploadRequestProtocolModal uploadRequestProtocolModal = new UploadRequestProtocolModal();

        private RequestDocumentManagementVM requestDocumentManagementVM = new RequestDocumentManagementVM();
        private IEnumerable<KeyValueVM> kvDocumentDocumentOperationTypeSource = new List<KeyValueVM>();
        private List<ProviderRequestDocumentVM> providerRequestDocumentsSource = new List<ProviderRequestDocumentVM>();
        private List<TypeOfRequestedDocumentVM> typeOfRequestDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private List<TypeOfRequestedDocumentVM> typeOfRequestDocumentsOtherCPOSource = new List<TypeOfRequestedDocumentVM>();
        private List<DocumentSerialNumberVM> addedFabricNumbersSource = new List<DocumentSerialNumberVM>();
        private bool requestNumberSelected = false;
        private IEnumerable<DocumentSeriesVM> documentSeriesSource = new List<DocumentSeriesVM>();
        private DocumentSerialNumberVM serialNumberToDelete = new DocumentSerialNumberVM();
        private RequestDocumentTypeVM requestDocumentTypeVM = new RequestDocumentTypeVM();
        private ValidationMessageStore? messageStore;
        private List<CandidateProviderVM> providerPartnersSource = new List<CandidateProviderVM>();
        private List<RequestDocumentManagementVM> requestManagementsFromOtherCPOSource = new List<RequestDocumentManagementVM>();
        private RequestDocumentManagementVM reqDocManagementFromPartnerVM = new RequestDocumentManagementVM();

        #region KV Document operation fields
        private IEnumerable<KeyValueVM> kvActionType;
        private KeyValueVM kvReceived;
        private KeyValueVM kvSubmitted;
        private KeyValueVM kvPrinted;
        private KeyValueVM kvCancelled;
        private KeyValueVM kvDestroyed;
        private KeyValueVM kvAwaitingConfirmation;
        private KeyValueVM kvLost;
        #endregion

        #region KV Request document status fields
        private IEnumerable<KeyValueVM> kvRequestDocumentStatus;
        private KeyValueVM kvCreated;
        private KeyValueVM kvFiledIn;
        private KeyValueVM kvProcessed;
        private KeyValueVM kvSummarized;
        private KeyValueVM kvFulfilled;
        #endregion

        #region KV Document request receive type
        private IEnumerable<KeyValueVM> kvDocumentRequestReceiveTypeSource;
        private KeyValueVM kvOtherCPO;
        private KeyValueVM kvMONPrinting;
        private bool showDeleteConfirmDialog = false;

        #endregion

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterModalSubmit { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.requestDocumentManagementVM);
        }

        private async Task LoadKVSources()
        {
            this.kvActionType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
            this.kvReceived = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Received");
            this.kvSubmitted = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.kvPrinted = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
            this.kvCancelled = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Cancelled");
            this.kvDestroyed = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Destroyed");
            this.kvAwaitingConfirmation = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "AwaitingConfirmation");
            this.kvLost = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Lost");

            this.kvRequestDocumentStatus = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestDocumetStatus");
            this.kvCreated = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Created");
            this.kvFiledIn = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.kvProcessed = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Processed");
            this.kvSummarized = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Summarized");
            this.kvFulfilled = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Fulfilled");

            this.kvDocumentRequestReceiveTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("DocumentRequestReceiveType");
            this.kvOtherCPO = this.kvDocumentRequestReceiveTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "OtherCPO");
            this.kvMONPrinting = this.kvDocumentRequestReceiveTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "PrintingHouse");
        }

        public async Task OpenModal(RequestDocumentManagementVM requestDocumentManagement, IEnumerable<KeyValueVM> kvDocumentRequestReceiveTypeSource, IEnumerable<DocumentSeriesVM> documentSeriesSource)
        {
            await this.LoadKVSources();
            this.providerPartnersSource.Clear();
            this.reqDocManagementFromPartnerVM = new RequestDocumentManagementVM();
            this.addedFabricNumbersSource.Clear();
            this.documentSeriesSource = documentSeriesSource;
            this.kvDocumentRequestReceiveTypeSource = kvDocumentRequestReceiveTypeSource;
            this.kvDocumentDocumentOperationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
            this.providerRequestDocumentsSource = (await this.ProviderDocumentRequestService.GetAllDocumentRequestsWhereStatusIsSummarizedAndByIdCandidateProviderAsync(requestDocumentManagement.IdCandidateProvider)).ToList();
            if (requestDocumentManagement.IdRequestDocumentManagement != 0)
            {
                this.requestDocumentManagementVM = await this.ProviderDocumentRequestService.GetRequestDocumentManagementByIdAsync(requestDocumentManagement.IdRequestDocumentManagement);
            }
            else
            {
                this.requestDocumentManagementVM = requestDocumentManagement;
            }

            this.editContext = new EditContext(this.requestDocumentManagementVM);
            this.requestDocumentManagementVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdModifyUser);
            this.requestDocumentManagementVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdCreateUser);
            if (this.requestDocumentManagementVM.IdRequestDocumentManagement != 0)
            {
                var typeOfDoc = await this.ProviderDocumentRequestService.GetTypeOfRequestedDocumentByIdAsync(new TypeOfRequestedDocumentVM() { IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument });
                this.typeOfRequestDocumentsSource.Add(typeOfDoc);
                this.requestDocumentTypeVM = await this.ProviderDocumentRequestService.GetRequestDocumentTypeByIdTypeOfRequestDocumentAndIdProviderRequestDocumentAsync(new RequestDocumentTypeVM() { IdProviderRequestDocument = requestDocumentManagementVM.IdProviderRequestDocument ?? default, IdTypeOfRequestedDocument = typeOfDoc.IdTypeOfRequestedDocument });
                this.addedFabricNumbersSource.AddRange(this.requestDocumentManagementVM.DocumentSerialNumbers.ToList());
                this.SetDataForFabricNumbersGrid();
            }

            if (this.requestDocumentManagementVM.IdCandidateProviderPartner is not null)
            {
                this.providerPartnersSource.Add(await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.requestDocumentManagementVM.IdCandidateProviderPartner.Value));
            }

            if (this.requestDocumentManagementVM.IdTypeOfRequestedDocument != 0)
            {
                this.typeOfRequestDocumentsOtherCPOSource.Add(await this.ProviderDocumentRequestService.GetTypeOfRequestedDocumentByIdAsync(new TypeOfRequestedDocumentVM() { IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument }));
            }

            if (this.requestDocumentManagementVM.IdDocumentRequestReceiveType is null)
            {
                this.requestDocumentManagementVM.IdDocumentRequestReceiveType = this.kvMONPrinting.IdKeyValue;
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task OpenUploadProtocolModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.uploadRequestProtocolModal.OpenModal(null, new RequestDocumentManagementUploadedFileVM() { IdRequestDocumentManagement = this.requestDocumentManagementVM.IdRequestDocumentManagement });
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterUploadModalSubmit()
        {
            this.requestDocumentManagementVM = await this.ProviderDocumentRequestService.GetRequestDocumentManagementByIdAsync(this.requestDocumentManagementVM.IdRequestDocumentManagement);

            this.editContext = new EditContext(this.requestDocumentManagementVM);
            this.requestDocumentManagementVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdModifyUser);
            this.requestDocumentManagementVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdCreateUser);

            await this.CallbackAfterModalSubmit.InvokeAsync();
        }

        private async Task OnRequestNumberValueChanged(ChangeEventArgs<int?, ProviderRequestDocumentVM> args)
        {
            this.requestDocumentManagementVM.IdTypeOfRequestedDocument = 0;
            if (args.Value != null)
            {
                this.requestNumberSelected = true;
                var requestNumber = this.providerRequestDocumentsSource.FirstOrDefault(x => x.IdProviderRequestDocument == args.Value).RequestNumber;
                this.typeOfRequestDocumentsSource = (await this.ProviderDocumentRequestService.GetTypesOfRequestedDocumentsByRequestNumberAsync(this.providerRequestDocumentsSource, requestNumber)).ToList();
                this.requestDocumentManagementVM.ProviderRequestDocument = this.providerRequestDocumentsSource.FirstOrDefault(x => x.IdProviderRequestDocument == this.requestDocumentManagementVM.IdProviderRequestDocument);
                this.requestDocumentManagementVM.IdCandidateProvider = this.requestDocumentManagementVM.ProviderRequestDocument.IdCandidateProvider;
                this.requestDocumentManagementVM.IdProviderRequestDocument = this.requestDocumentManagementVM.ProviderRequestDocument.IdProviderRequestDocument;
                this.requestDocumentManagementVM.ReceiveDocumentYear = this.requestDocumentManagementVM.ProviderRequestDocument.CurrentYear ?? default;
            }
            else
            {
                this.requestNumberSelected = false;
                this.typeOfRequestDocumentsSource.Clear();
                this.requestDocumentManagementVM.ProviderRequestDocument = null;
                this.requestDocumentManagementVM.IdCandidateProvider = 0;
                this.requestDocumentManagementVM.IdProviderRequestDocument = null;
                this.requestDocumentManagementVM.ReceiveDocumentYear = 0;
                this.requestDocumentManagementVM.ReceiveDocumentYear = 0;
            }
        }

        private async Task OnTypeOfRequestedDocumentValueChanged(ChangeEventArgs<int, TypeOfRequestedDocumentVM> args)
        {
            if (args.Value != 0)
            {
                this.requestDocumentTypeVM = await this.ProviderDocumentRequestService.GetRequestDocumentTypeByIdTypeOfRequestDocumentAndIdProviderRequestDocumentAsync(new RequestDocumentTypeVM() { IdProviderRequestDocument = requestDocumentManagementVM.IdProviderRequestDocument ?? default, IdTypeOfRequestedDocument = args.Value });
                this.requestDocumentManagementVM.DocumentCount = this.requestDocumentTypeVM != null ? this.requestDocumentTypeVM.DocumentCount : 0;
            }
            else
            {
                this.requestDocumentTypeVM = new RequestDocumentTypeVM();
                this.requestDocumentManagementVM.DocumentCount = 0;
            }
        }

        private async Task OnAddNumberClickHandler()
        {
            if (string.IsNullOrEmpty(this.requestDocumentManagementVM.SerialNumber))
            {
                await this.ShowErrorAsync("Моля, въведете фабричен номер!");
                return;
            }

            this.requestDocumentManagementVM.SerialNumber = this.requestDocumentManagementVM.SerialNumber.Trim();

            if (!this.CheckIfSerialNumberInputIsNumber())
            {
                await this.ShowErrorAsync("Моля, въведете число за стойност на фабричен номер!");
                return;
            }

            if (this.addedFabricNumbersSource.Any(x => x.SerialNumber == this.requestDocumentManagementVM.SerialNumber))
            {
                await this.ShowErrorAsync("Този фабричен номер е вече добавен!");
                return;
            }

            if (this.requestDocumentManagementVM.DocumentCount == this.addedFabricNumbersSource.Count)
            {
                await this.ShowErrorAsync("Въведените фабрични номера не могат да надвишават общия въведен брой получени документи!");
                return;
            }

            DocumentSerialNumberVM documentSerialNumber = new DocumentSerialNumberVM()
            {
                IdDocumentOperation = this.kvReceived.IdKeyValue,
                IdRequestDocumentManagement = this.requestDocumentManagementVM.IdRequestDocumentManagement,
                //IdCandidateProvider = this.requestDocumentManagementVM.ProviderRequestDocument.IdCandidateProvider,
                IdCandidateProvider = this.UserProps.IdCandidateProvider,
                IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument,
                DocumentDate = this.requestDocumentManagementVM.DocumentDate.Value,
                SerialNumber = this.requestDocumentManagementVM.SerialNumber
            };

            var result = await this.ProviderDocumentRequestService.AddDocumentSerialNumberToRequestDocumentManagementAsync(documentSerialNumber);
            if (result.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
            }
            else
            {
                await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                this.addedFabricNumbersSource = (await this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdRequestDocumentManagementAsync(this.requestDocumentManagementVM.IdRequestDocumentManagement)).ToList();
                this.SetDataForFabricNumbersGrid();
                this.requestDocumentManagementVM.SerialNumber = string.Empty;

                await this.CallbackAfterModalSubmit.InvokeAsync();

                this.StateHasChanged();
            }
        }

        private bool CheckIfSerialNumberInputIsNumber()
        {
            for (int i = 0; i < this.requestDocumentManagementVM.SerialNumber.Length; i++)
            {
                var charEl = this.requestDocumentManagementVM.SerialNumber[i];
                if (!char.IsDigit(charEl))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task DeleteSerialNumber(DocumentSerialNumberVM documentSerialNumber)
        {
            this.serialNumberToDelete = documentSerialNumber;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {

                var inputContext = new ResultContext<DocumentSerialNumberVM>();
                inputContext.ResultContextObject = this.serialNumberToDelete;
                var resultContext = await this.ProviderDocumentRequestService.DeleteDocumentSerialNumberAsync(inputContext);

                if (resultContext.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                    this.addedFabricNumbersSource = (await this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdRequestDocumentManagementAsync(this.requestDocumentManagementVM.IdRequestDocumentManagement)).ToList();
                    this.SetDataForFabricNumbersGrid();

                    await this.CallbackAfterModalSubmit.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }

        private void SetDataForFabricNumbersGrid()
        {
            foreach (var entry in this.addedFabricNumbersSource)
            {
                var operationType = this.kvDocumentDocumentOperationTypeSource.FirstOrDefault(x => x.IdKeyValue == entry.IdDocumentOperation);
                if (operationType is not null)
                {
                    entry.DocumentOperationName = operationType.Name;
                }

                entry.ReceiveDocumentYear = this.requestDocumentManagementVM.ReceiveDocumentYear.Value;

                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == entry.IdTypeOfRequestedDocument && x.Year == entry.ReceiveDocumentYear);
                if (docSeries is not null)
                {
                    entry.DocumentSeriesName = docSeries.SeriesName;
                }
            }

            this.addedFabricNumbersSource = this.addedFabricNumbersSource.OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();
        }

        private async Task Save()
        {
            this.editContext = new EditContext(this.requestDocumentManagementVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateDocumentCountWithDocumentSerialNumbersAdded;

            this.requestDocumentManagementVM.IdDocumentOperation = this.kvReceived.IdKeyValue;

            if (this.editContext.Validate())
            {
                this.SpinnerShow();

                if (this.requestDocumentManagementVM.DocumentSerialNumbers != null)
                {
                    this.requestDocumentManagementVM.DocumentSerialNumbers = this.addedFabricNumbersSource;
                }

                ResultContext<RequestDocumentManagementVM> inputContext = new ResultContext<RequestDocumentManagementVM>();
                inputContext.ResultContextObject = this.requestDocumentManagementVM;

                var result = new ResultContext<RequestDocumentManagementVM>();

                if (this.requestDocumentManagementVM.IdRequestDocumentManagement != 0)
                {
                    if (this.requestDocumentManagementVM.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue)
                    {
                        result = await this.ProviderDocumentRequestService.UpdateRequestDocumentManagementAsync(inputContext, false, true);
                    }
                    else
                    {
                        result = await this.ProviderDocumentRequestService.UpdateRequestDocumentManagementAsync(inputContext);
                    }
                }
                else
                {
                    result = await this.ProviderDocumentRequestService.CreateRequestDocumentManagementAsync(inputContext);
                }

                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    this.requestDocumentManagementVM.IdRequestDocumentManagement = result.ResultContextObject.IdRequestDocumentManagement;
                    this.requestDocumentManagementVM.TypeOfRequestedDocument = this.typeOfRequestDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == this.requestDocumentManagementVM.IdTypeOfRequestedDocument);

                    if (this.requestDocumentManagementVM.IdDocumentRequestReceiveType == this.kvMONPrinting.IdKeyValue)
                    {
                        if (this.requestDocumentTypeVM != null)
                        {
                            if (this.requestDocumentTypeVM.IdRequestDocumentType == 0)
                            {
                                this.requestDocumentTypeVM = await this.ProviderDocumentRequestService.GetRequestDocumentTypeByIdTypeOfRequestDocumentAndIdProviderRequestDocumentAsync(new RequestDocumentTypeVM() { IdProviderRequestDocument = requestDocumentManagementVM.IdProviderRequestDocument ?? default, IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument });
                            }

                            this.requestDocumentTypeVM.IdRequestDocumentManagement = this.requestDocumentManagementVM.IdRequestDocumentManagement;
                            await this.ProviderDocumentRequestService.UpdateRequestDocumentTypeAsync(this.requestDocumentTypeVM);
                        }
                    }

                    this.requestDocumentManagementVM = await this.ProviderDocumentRequestService.GetRequestDocumentManagementByIdAsync(this.requestDocumentManagementVM.IdRequestDocumentManagement);
                    this.addedFabricNumbersSource.AddRange(this.requestDocumentManagementVM.DocumentSerialNumbers.ToList());

                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    await this.CallbackAfterModalSubmit.InvokeAsync();

                    this.StateHasChanged();
                }
            }
            else
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
            }

            this.SpinnerHide();
        }

        private async Task OnAddConsecutiveNumbersClickHandler()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.requestDocumentManagementVM.DocumentCount == this.addedFabricNumbersSource.Count)
                {
                    await this.ShowErrorAsync("Въведените фабрични номера не могат да надвишават общия въведен брой получени документи!");
                }
                else
                {
                    this.cpoConsecutiveNumbersModal.OpenModal(this.addedFabricNumbersSource, this.requestDocumentManagementVM);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterConsecutiveNumbersAdd()
        {
            this.addedFabricNumbersSource = (await this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdRequestDocumentManagementAsync(this.requestDocumentManagementVM.IdRequestDocumentManagement)).ToList();
            this.SetDataForFabricNumbersGrid();
            this.StateHasChanged();

            await this.CallbackAfterModalSubmit.InvokeAsync();
        }

        private void ValidateDocumentCountWithDocumentSerialNumbersAdded(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.requestDocumentManagementVM.IdDocumentRequestReceiveType is null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "IdDocumentRequestReceiveType");
                this.messageStore?.Add(fi, "Полето 'Получен от' е задължително!");
            }

            if (this.requestDocumentManagementVM.IdDocumentRequestReceiveType == this.kvMONPrinting.IdKeyValue)
            {
                if (this.requestDocumentManagementVM.IdProviderRequestDocument is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "IdProviderRequestDocument");
                    this.messageStore?.Add(fi, "Полето '№ на заявка' е задължително!");
                }

                if (this.requestDocumentManagementVM.DocumentCount > this.requestDocumentTypeVM.DocumentCount)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                    this.messageStore?.Add(fi, "Въведеният брой на получени документи не може да надвишава броя на заявени документи!");
                }
            }
            else
            {
                if (this.requestDocumentManagementVM.DocumentCount < 1)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                    this.messageStore?.Add(fi, "Въведеният брой на получени документи не може да бъде по-малък от 1!");
                }
            }

            if (this.requestDocumentManagementVM.DocumentCount < this.addedFabricNumbersSource.Count)
            {
                FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                this.messageStore?.Add(fi, "Въведените фабрични номера не могат да надвишават общия въведен брой получени документи!");
            }
        }

        private async Task ReceivedFromValueChanged(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.Value.Value == this.kvDocumentRequestReceiveTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "OtherCPO").IdKeyValue)
            {
                RequestDocumentManagementVM requestDocumentManagementVM = new RequestDocumentManagementVM()
                {
                    IdCandidateProvider = this.UserProps.IdCandidateProvider
                };

                this.requestManagementsFromOtherCPOSource = (await this.ProviderDocumentRequestService.GetAllRequestDocumentManagementsByIdProviderAndByDocumentOperationAwaitingConfirmationAsync(requestDocumentManagementVM)).ToList();
                if (this.requestManagementsFromOtherCPOSource.Any())
                {
                    var listProviderPartnerIds = this.requestManagementsFromOtherCPOSource.Select(x => x.IdCandidateProviderPartner.Value).ToList();

                    this.providerPartnersSource = (await this.CandidateProviderService.GetCandidateProvidersByListIdsAsync(listProviderPartnerIds)).ToList();
                }
            }
            else
            {
                this.providerPartnersSource.Clear();
            }
        }

        private async Task OnProviderPartnerSelected(ChangeEventArgs<int?, CandidateProviderVM> args)
        {
            var listIds = this.requestManagementsFromOtherCPOSource.Select(x => x.IdTypeOfRequestedDocument).ToList();
            this.typeOfRequestDocumentsOtherCPOSource = (await this.ProviderDocumentRequestService.GetTypesOfRequestedDocumentByListIdsAsync(listIds)).ToList();
        }

        private async Task OnTypeOfRequestedDocumentOtherCPOValueChanged(ChangeEventArgs<int, TypeOfRequestedDocumentVM> args)
        {
            if (args.Value != 0)
            {
                RequestDocumentManagementVM request = new RequestDocumentManagementVM()
                {
                    IdCandidateProvider = this.UserProps.IdCandidateProvider,
                    IdCandidateProviderPartner = this.requestDocumentManagementVM.IdCandidateProviderPartner,
                    IdTypeOfRequestedDocument = args.Value
                };

                this.reqDocManagementFromPartnerVM = await this.ProviderDocumentRequestService.GetRequestDocumentManagementByProviderIdByProviderPartnerIdByIdTypeOfRequestedDocumentAndByDocumentOperationAwaitingConfirmationAsync(request);
                this.requestDocumentManagementVM.IdRequestDocumentManagement = reqDocManagementFromPartnerVM.IdRequestDocumentManagement;
                this.requestDocumentManagementVM.DocumentCount = reqDocManagementFromPartnerVM.DocumentCount;
                this.requestDocumentManagementVM.DocumentDate = reqDocManagementFromPartnerVM.DocumentDate;
                this.requestDocumentManagementVM.ReceiveDocumentYear = reqDocManagementFromPartnerVM.ReceiveDocumentYear;
                this.requestDocumentManagementVM.IdDocumentOperation = reqDocManagementFromPartnerVM.IdDocumentOperation;
                this.addedFabricNumbersSource = reqDocManagementFromPartnerVM.DocumentSerialNumbers.Where(x => x.IdDocumentOperation == this.kvAwaitingConfirmation.IdKeyValue).ToList();
                this.SetDataForFabricNumbersGrid();
                this.addedFabricNumbersGrid.Refresh();
            }
            else
            {
                this.addedFabricNumbersSource.Clear();
            }
        }

        private async Task OnRemove(string fileName)
        {
            var uploadedFile = this.requestDocumentManagementVM.RequestDocumentManagementUploadedFiles.FirstOrDefault();
            if (uploadedFile is not null)
            {
                if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                {
                    this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                    this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;

                }
            }
        }
        public async void ConfirmDeleteCallback()
        {
            var uploadedFile = this.requestDocumentManagementVM.RequestDocumentManagementUploadedFiles.FirstOrDefault();
            this.showDeleteConfirmDialog = false;
            int result = result = await this.UploadFileService.RemoveFileByIdAsync<RequestDocumentManagementUploadedFile>(uploadedFile.IdRequestDocumentManagementUploadedFile);
            if (result == 1)
            {
                uploadedFile.UploadedFileName = string.Empty;

                await this.CallbackAfterModalSubmit.InvokeAsync();
            }

            this.StateHasChanged();

        }
        private async void OnDownloadClick()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var uploadedFile = this.requestDocumentManagementVM.RequestDocumentManagementUploadedFiles.FirstOrDefault();
                if (uploadedFile is not null)
                {
                    if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<RequestDocumentManagementUploadedFile>(uploadedFile.IdRequestDocumentManagementUploadedFile);
                        if (hasFile)
                        {
                            var documentStream = await this.UploadFileService.GetUploadedFileAsync<RequestDocumentManagementUploadedFile>(uploadedFile.IdRequestDocumentManagementUploadedFile);
                            if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                            {
                                await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                            }
                            else
                            {
                                await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, documentStream.MS!.ToArray());
                            }
                        }
                        else
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await this.ShowErrorAsync(msg);
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                        await this.ShowErrorAsync(msg);
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
