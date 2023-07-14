using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class HandingOverDocumentModal : BlazorBaseComponent
    {
        private SfDialog handingOverDocumentModal = new SfDialog();
        private ToastMsg toast = new ToastMsg();
        private SfAutoComplete<int?, CandidateProviderVM> providersAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private SfGrid<DocumentSerialNumberVM> addedFabricNumbersGrid = new SfGrid<DocumentSerialNumberVM>();
        private HandingOverDocumentAddSerialNumberModal handingOverDocumentAddSerialNumberModal = new HandingOverDocumentAddSerialNumberModal();

        private RequestDocumentManagementVM requestDocumentManagementVM = new RequestDocumentManagementVM();
        private List<CandidateProviderVM> providersSource = new List<CandidateProviderVM>();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private List<RequestDocumentManagementVM> receivedReqDocManagementsSource = new List<RequestDocumentManagementVM>();
        private List<int?> yearsSource = new List<int?>();
        private List<DocumentSerialNumberVM> addedFabricNumbersSource = new List<DocumentSerialNumberVM>();
        private List<DocumentSeriesVM> documentSeriesSource = new List<DocumentSeriesVM>();
        private IEnumerable<KeyValueVM> kvDocumentOperationsSource = new List<KeyValueVM>();
        private int documentCount = 0;
        private DocumentSerialNumberVM documentSerialNumberToDelete = new DocumentSerialNumberVM();
        private ValidationMessageStore? messageStore;
        private bool hasSerialNumber = false;

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.requestDocumentManagementVM);
        }

        public async Task OpenModal(RequestDocumentManagementVM requestDocumentManagement, List<DocumentSeriesVM> documentSeriesSource)
        {
            this.documentSeriesSource = documentSeriesSource.ToList();
            this.addedFabricNumbersSource.Clear();
            this.documentCount = 0;
            this.typeOfRequestedDocumentsSource.Clear();
            this.receivedReqDocManagementsSource = (await this.ProviderDocumentRequestService.GetAllRequestDocumentManagementsByDocumentOperationReceivedAndByIdCandidateProviderAsync(requestDocumentManagement.IdCandidateProvider)).ToList();
            foreach (var entry in this.receivedReqDocManagementsSource)
            {
                if (!this.yearsSource.Contains(entry.ReceiveDocumentYear))
                {
                    this.yearsSource.Add(entry.ReceiveDocumentYear);
                }
            }
            
            this.providersSource = (await this.CandidateProviderService.GetAllActiveCandidateProvidersWithoutAnythingIncludedAsync(new CandidateProviderVM() { IsActive = true })).ToList();
            this.kvDocumentOperationsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");

            this.requestDocumentManagementVM = requestDocumentManagement;

            if (requestDocumentManagement.IdTypeOfRequestedDocument != 0)
            {
                this.typeOfRequestedDocumentsSource.Add(await this.ProviderDocumentRequestService.GetTypeOfRequestedDocumentByIdAsync(new TypeOfRequestedDocumentVM() { IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument }));
                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == this.requestDocumentManagementVM.IdTypeOfRequestedDocument && x.Year == this.requestDocumentManagementVM.ReceiveDocumentYear);
                if (docSeries is not null)
                {
                    this.hasSerialNumber = true;
                }

                this.GetDocumentCountFromDB();
                //this.documentCount -= this.requestDocumentManagementVM.DocumentCount;
            }

            if (this.requestDocumentManagementVM.DocumentSerialNumbers.Any())
            {
                var kvSubmitted = this.kvDocumentOperationsSource.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
                this.addedFabricNumbersSource = this.requestDocumentManagementVM.DocumentSerialNumbers.ToList();
                this.addedFabricNumbersSource = this.addedFabricNumbersSource.Where(x => x.IdDocumentOperation == kvSubmitted.IdKeyValue).ToList();
                if (this.addedFabricNumbersSource.Any())
                {
                    this.SetDataForSerialNumbersGrid();
                }
            }

            this.editContext = new EditContext(this.requestDocumentManagementVM);
            if (this.requestDocumentManagementVM.IdRequestDocumentManagement == 0)
            {
                this.requestDocumentManagementVM.ModifyDate = DateTime.Now;
                this.requestDocumentManagementVM.CreationDate = DateTime.Now;
                this.requestDocumentManagementVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.UserProps.UserId);
                this.requestDocumentManagementVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.UserProps.UserId);
            }
            else
            {
                this.requestDocumentManagementVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdModifyUser);
                this.requestDocumentManagementVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdCreateUser);
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void SetDataForSerialNumbersGrid()
        {
            foreach (var docSerialNumber in this.addedFabricNumbersSource)
            {
                var typeOfReqDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                if (typeOfReqDoc is not null)
                {
                    docSerialNumber.TypeOfRequestedDocument = typeOfReqDoc;
                }

                docSerialNumber.ReceiveDocumentYear = this.requestDocumentManagementVM.ReceiveDocumentYear.Value;

                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument && x.Year == docSerialNumber.ReceiveDocumentYear);
                if (docSeries is not null)
                {
                    docSerialNumber.DocumentSeriesName = docSeries.SeriesName;
                }

                var actionType = this.kvDocumentOperationsSource.FirstOrDefault(x => x.IdKeyValue == docSerialNumber.IdDocumentOperation);
                if (actionType is not null)
                {
                    docSerialNumber.DocumentOperationName = actionType.Name;
                }
            }
        }

        private async Task Save()
        {
            string msg = "Сигурни ли сте, че искате да предадете документи към избраното ЦПО? След потвърждаване няма да можете да извършвате промени в попълнената информация.";
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

                    if (this.requestDocumentManagementVM.IdRequestDocumentManagement == 0)
                    {
                            await this.SaveDataAsync();                        
                    }
                    else
                    {
                        await this.SaveDataAsync();
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task SaveDataAsync()
        {
            this.editContext = new EditContext(this.requestDocumentManagementVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.editContext.OnValidationRequested += ValidateDocumentCount;
            this.messageStore = new ValidationMessageStore(this.editContext);

            if (this.editContext.Validate())
            {
                this.SpinnerShow();
                var kvActionType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                var inputContext = new ResultContext<RequestDocumentManagementVM>();
                this.requestDocumentManagementVM.IdCandidateProvider = this.requestDocumentManagementVM.IdCandidateProvider;
                this.requestDocumentManagementVM.IdDocumentOperation = kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Submitted").IdKeyValue;
                this.requestDocumentManagementVM.DocumentSerialNumbers.Clear();
                this.requestDocumentManagementVM.DocumentSerialNumbers = this.addedFabricNumbersSource.ToList();

                inputContext.ResultContextObject = this.requestDocumentManagementVM;

                var result = new ResultContext<RequestDocumentManagementVM>();

                if (this.requestDocumentManagementVM.IdRequestDocumentManagement == 0)
                {
                    result = await this.ProviderDocumentRequestService.CreateRequestDocumentManagementAsync(inputContext);
                }
                else
                {
                    result = await this.ProviderDocumentRequestService.UpdateRequestDocumentManagementAsync(inputContext, true);
                }

                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    this.requestDocumentManagementVM = result.ResultContextObject;
                    this.requestDocumentManagementVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdModifyUser);
                    this.requestDocumentManagementVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestDocumentManagementVM.IdCreateUser);
                    //this.documentCount -= this.requestDocumentManagementVM.DocumentCount;
                    this.GetDocumentCountFromDB();
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    this.StateHasChanged();
                    await this.CallbackAfterSubmit.InvokeAsync();
                }
            }
            else
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
            }
        }

        private async Task OnFilterProvider(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                var query = new Query().Where(new WhereFilter() { Field = "CPONameAndOwnerName", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.providersAutoComplete.FilterAsync(this.providersSource, query);
            }
        }

        private async Task LoadTypeOfDocumentsData(ChangeEventArgs<int?, int?> args)
        {
            if (args.Value != 0)
            {
                var reqDocManagementsByYearSource = this.receivedReqDocManagementsSource.Where(x => x.ReceiveDocumentYear == this.requestDocumentManagementVM.ReceiveDocumentYear).ToList();
                var listIds = reqDocManagementsByYearSource.Select(x => x.IdTypeOfRequestedDocument).ToList();
                if (listIds.Any())
                {
                    this.typeOfRequestedDocumentsSource = (await this.ProviderDocumentRequestService.GetTypesOfRequestedDocumentByListIdsAsync(listIds)).ToList();
                }
                else
                {
                    this.typeOfRequestedDocumentsSource.Clear();
                }
            }
            else
            {
                this.typeOfRequestedDocumentsSource.Clear();
            }
        }

        private void GetDocumentCount(ChangeEventArgs<int, TypeOfRequestedDocumentVM> args)
        {
            if (args.Value != 0)
            {
                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == this.requestDocumentManagementVM.IdTypeOfRequestedDocument && x.Year == this.requestDocumentManagementVM.ReceiveDocumentYear);
                if (docSeries is not null)
                {
                    this.hasSerialNumber = true;
                }
                else
                {
                    this.hasSerialNumber = false;
                }

                this.requestDocumentManagementVM.DocumentCount = 0;
                this.GetDocumentCountFromDB();
            }
            else
            {
                this.documentCount = 0;
                this.requestDocumentManagementVM.DocumentCount = 0;
            }
        }

        private void GetDocumentCountFromDB()
        {
            RequestDocumentManagementVM requestDocumentManagement = new RequestDocumentManagementVM()
            {
                IdCandidateProvider = this.requestDocumentManagementVM.IdCandidateProvider,
                IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument,
            };

            this.documentCount = this.ProviderDocumentRequestService.GetDocumentCountByDocumentOperationReceivedByProviderIdByTypeOfRequestedDocumentIdAndReceiveYear(requestDocumentManagementVM);
        }

        private async Task OnAddDocumentSerialNumbersClickHandler()
        {
            var documentSerialNumbers = (await this.ProviderDocumentRequestService.GetDocumentSerialNumbersWithOperationReceivedAndByTypeOfRequestedDocumentIdAndByIdCandidateProviderAsync(new TypeOfRequestedDocumentVM() { IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument }, this.requestDocumentManagementVM.IdCandidateProvider)).ToList();
            if (this.addedFabricNumbersSource.Any())
            {
                foreach (var docSerialNumber in this.addedFabricNumbersSource)
                {
                    documentSerialNumbers.RemoveAll(x => x.SerialNumber == docSerialNumber.SerialNumber);
                }
            }

            foreach (var docSerialNumber in documentSerialNumbers)
            {
                var typeOfReqDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                if (typeOfReqDoc is not null)
                {
                    docSerialNumber.TypeOfRequestedDocument = typeOfReqDoc;
                }

                docSerialNumber.ReceiveDocumentYear = this.requestDocumentManagementVM.ReceiveDocumentYear.Value;

                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument && x.Year == docSerialNumber.ReceiveDocumentYear);
                if (docSeries is not null)
                {
                    docSerialNumber.DocumentSeriesName = docSeries.SeriesName;
                }
            }

            this.handingOverDocumentAddSerialNumberModal.OpenModal(documentSerialNumbers);
        }

        private async Task UpdateAfterFabricNumbersAdded(List<DocumentSerialNumberVM> documentSerialNumbers)
        {
            this.addedFabricNumbersSource.AddRange(documentSerialNumbers);

            foreach (var docSerialNumber in this.addedFabricNumbersSource)
            {
                var typeOfReqDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                if (typeOfReqDoc is not null)
                {
                    docSerialNumber.TypeOfRequestedDocument = typeOfReqDoc;
                }

                docSerialNumber.ReceiveDocumentYear = this.requestDocumentManagementVM.ReceiveDocumentYear.Value;

                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument && x.Year == docSerialNumber.ReceiveDocumentYear);
                if (docSeries is not null)
                {
                    docSerialNumber.DocumentSeriesName = docSerialNumber.DocumentSeriesName;
                }

                var actionType = this.kvDocumentOperationsSource.FirstOrDefault(x => x.IdKeyValue == docSerialNumber.IdDocumentOperation);
                if (actionType is not null)
                {
                    docSerialNumber.DocumentOperationName = actionType.Name;
                }

                docSerialNumber.DocumentOperationName = "Предаден";
            }

            await this.addedFabricNumbersGrid.Refresh();
        }

        private async Task DeleteDocumentSerialNumber(DocumentSerialNumberVM documentSerialNumber)
        {
            if (this.requestDocumentManagementVM.DocumentCount == this.addedFabricNumbersSource.Count)
            {
                this.toast.sfErrorToast.Content = "Общият въведен брой предадени документи не може да бъде различен от броя въведени фабрични номера!";
                await this.toast.sfErrorToast.ShowAsync();
                return;
            }

            this.documentSerialNumberToDelete = documentSerialNumber;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {

                this.documentSerialNumberToDelete.IdRequestDocumentManagement = this.requestDocumentManagementVM.IdRequestDocumentManagement;
                var serialNumbersToDelete = await this.ProviderDocumentRequestService.GetDocumentSerialNumbersWithOperationSubmittedAndOperationAwaitingConfirmationAndSerialNumberAsync(this.documentSerialNumberToDelete, this.requestDocumentManagementVM.IdCandidateProviderPartner.Value);
                if (serialNumbersToDelete.Any())
                {
                    var listIds = serialNumbersToDelete.Select(x => x.IdDocumentSerialNumber).ToList();

                    var result = await this.ProviderDocumentRequestService.DeleteDocumentSerialNumbersByListIdsAsync(listIds);
                    if (result.HasErrorMessages)
                    {
                        this.toast.sfErrorToast.Content = string.Join(Environment.NewLine, result.ListErrorMessages);
                        await this.toast.sfErrorToast.ShowAsync();
                        return;
                    }
                    else
                    {
                        DocumentSerialNumberVM docSerialNumber = new DocumentSerialNumberVM()
                        {
                            IdCandidateProvider = this.requestDocumentManagementVM.IdCandidateProvider,
                            IdRequestDocumentManagement = this.requestDocumentManagementVM.IdRequestDocumentManagement
                        };

                        var docSerialNumbers = await this.ProviderDocumentRequestService.GetDocumentSerialNumbersWithOperationSubmittedByRequestDocumentManagementIdAndProviderIdAsync(docSerialNumber);

                        foreach (var item in docSerialNumbers)
                        {
                            if (!this.addedFabricNumbersSource.Any(x => x.IdDocumentSerialNumber == item.IdDocumentSerialNumber))
                            {
                                this.addedFabricNumbersSource.Add(item);
                            }
                        }
                    }
                }

                this.addedFabricNumbersSource.RemoveAll(x => x.IdDocumentSerialNumber == this.documentSerialNumberToDelete.IdDocumentSerialNumber);

                this.addedFabricNumbersGrid.Refresh();

                this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                await this.toast.sfSuccessToast.ShowAsync();
            }
        }

        private void ValidateDocumentCount(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.requestDocumentManagementVM.IdRequestDocumentManagement != 0)
            {
                if (this.requestDocumentManagementVM.DocumentCount == 0)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                    this.messageStore?.Add(fi, "Полето 'Брой' трябва да има стойност по-голяма от 0!");
                    return;
                }
            }

            if (this.addedFabricNumbersSource.Count > this.requestDocumentManagementVM.DocumentCount)
            {
                FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                this.messageStore?.Add(fi, "Въведените фабрични номера не могат да надвишават общия въведен брой предадени документи!");
            }

            if (this.requestDocumentManagementVM.DocumentCount > this.documentCount)
            {
                FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                this.messageStore?.Add(fi, "Общият въведен брой предадени документи не може да надвишава броя получени документи!");
            }

            var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == this.requestDocumentManagementVM.IdTypeOfRequestedDocument && x.Year == this.requestDocumentManagementVM.ReceiveDocumentYear);
            if (docSeries is null)
            {
                return;
            }

            if (this.requestDocumentManagementVM.IdRequestDocumentManagement != 0)
            {
                if (this.requestDocumentManagementVM.DocumentCount != this.addedFabricNumbersSource.Count)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                    this.messageStore?.Add(fi, "Общият въведен брой предадени документи не може да бъде различен от броя въведени фабрични номера!");
                }
            }
        }
    }
}
