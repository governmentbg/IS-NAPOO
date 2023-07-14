using Data.Models.Data.Candidate;
using Data.Models.Data.Request;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Pages.Request.CPO.CPODocumentDestructionAddSerialNumbersModal;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPODocumentDestructionModal : BlazorBaseComponent
    {
        private SfGrid<DocumentSerialNumberVM> addedFabricNumbersGrid = new SfGrid<DocumentSerialNumberVM>();
        private SfGrid<DocumentSerialNumberVM> handedOverDocumentsGrid = new SfGrid<DocumentSerialNumberVM>();
        private SfGrid<DocumentSerialNumberVM> printedDocumentsGrid = new SfGrid<DocumentSerialNumberVM>();
        private SfGrid<DocumentSerialNumberVM> availableDocumentsGrid = new SfGrid<DocumentSerialNumberVM>();
        private SfGrid<ReportUploadedDocVM> reportUploadedDocGrid = new SfGrid<ReportUploadedDocVM>();
        private ToastMsg toast = new ToastMsg();
        private SfDialog cpoDocumentDestructionModal = new SfDialog();
        private CPODocumentDestructionAddSerialNumbersModal cpoDocumentDestructionAddSerialNumbersModal = new CPODocumentDestructionAddSerialNumbersModal();
        private CPODocumentDestructionDocumentModal cpoDocumentDestructionDocumentModal = new CPODocumentDestructionDocumentModal();

        private List<DocumentSerialNumberVM> addedFabricNumbersSource = new List<DocumentSerialNumberVM>();
        private List<DocumentSerialNumberVM> handedOverDocumentsSource = new List<DocumentSerialNumberVM>();
        private List<DocumentSerialNumberVM> printedDocumentsSource = new List<DocumentSerialNumberVM>();
        private List<DocumentSerialNumberVM> availableDocumentsSource = new List<DocumentSerialNumberVM>();
        private List<ReportUploadedDocVM> reportUploadedDocSource = new List<ReportUploadedDocVM>();
        private RequestReportVM requestReportVM = new RequestReportVM();
        private IEnumerable<KeyValueVM> kvRequestReportStatusSource = new List<KeyValueVM>();
        private DocumentSerialNumberVM documentSerialNumberToDelete = new DocumentSerialNumberVM();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private ReportUploadedDocVM documentToDelete = new ReportUploadedDocVM();
        private ValidationMessageStore? messageStore;
        private List<string> validationMessages = new List<string>();
        private string reportPeriod = string.Empty;
        private bool isInRoleNAPOOExpert = false;
        private KeyValueVM kvReportStatusSubmittedValue = new KeyValueVM();
        private KeyValueVM kvReportStatusReturnedValue = new KeyValueVM();

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

        #region KV Document report type
        IEnumerable<KeyValueVM> kvDocumentTypeSource;
        KeyValueVM kvReport;
        KeyValueVM kvDestroying;
        KeyValueVM kvProtocolOfAcceptanceAndTransfer;
        #endregion

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        public async Task OpenModal(RequestReportVM requestReport, IEnumerable<KeyValueVM> kvRequestReportStatusSource, bool? isInRoleNAPOO = null)
        {
            await this.LoadKVSources();
            this.validationMessages.Clear();
            this.reportUploadedDocSource.Clear();
            this.typeOfRequestedDocumentsSource = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();
            this.kvRequestReportStatusSource = kvRequestReportStatusSource.ToList();
            this.kvReportStatusReturnedValue = this.kvRequestReportStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Returned");
            this.kvReportStatusSubmittedValue = this.kvRequestReportStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.requestReportVM = requestReport;
            this.editContext = new EditContext(this.requestReportVM);
            this.SetFabricNumbersDataAtOpenModal();
            await this.SetDataForUploadedDocsGrid();

            await this.LoadDataForHandedOverDocumentsAsync();

            await this.LoadDataForPrintedDocumentsAsync();

            await this.LoadDataForAvailableDocuments();

            this.reportPeriod = (await this.DataSourceService.GetSettingByIntCodeAsync("DocumentReportPeriod")).SettingValue;

            if (isInRoleNAPOO.HasValue)
            {
                this.isInRoleNAPOOExpert = isInRoleNAPOO.Value;
            }

            if (requestReport.IdRequestReport != 0)
            {
                await this.SetCreateAndModifyInfoAsync();
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SetDataForUploadedDocsGrid()
        {
            this.reportUploadedDocSource = this.requestReportVM.ReportUploadedDocs.ToList();
            foreach (var uploadedDoc in this.reportUploadedDocSource)
            {
                uploadedDoc.TypeReportUploadedDocumentName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == uploadedDoc.IdTypeReportUploadedDocument).Name;
                uploadedDoc.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(uploadedDoc.IdCreateUser);

                var typeOfDocument = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == uploadedDoc.IdTypeReportUploadedDocument);
                if (typeOfDocument is not null)
                {
                    uploadedDoc.TypeReportUploadedDocumentName = typeOfDocument.Name;
                }
            }

            this.StateHasChanged();
        }

        private void SetFabricNumbersDataAtOpenModal()
        {
            this.addedFabricNumbersSource = this.requestReportVM.DocumentSerialNumbers.OrderBy(x => x.SerialNumber).ToList();
            foreach (var docSerialNumber in this.addedFabricNumbersSource)
            {
                var typeOfDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                if (typeOfDoc is not null)
                {
                    docSerialNumber.TypeOfRequestedDocument = typeOfDoc;
                }

                var docOperation = this.kvActionType.FirstOrDefault(x => x.IdKeyValue == docSerialNumber.IdDocumentOperation);
                if (docOperation is not null)
                {
                    docSerialNumber.DocumentOperationName = docOperation.Name;
                }
            }
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

            this.kvDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeReportUploadedDocument");
            this.kvReport = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ProtocolFactoryNumbering");
            this.kvDestroying = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DestroyingFactoryNumbers");
            this.kvProtocolOfAcceptanceAndTransfer = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "AcceptanceAndTransferProtocolForDocumentsWithFactoryNumbering");
        }

        private async Task SaveBtn(bool showToastMessage)
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateReportYear;

                if (this.editContext.Validate())
                {
                    if (this.requestReportVM.IdRequestReport == 0)
                    {
                        // проверява дали вече има създаден отчет за тази календарна година
                        if (await this.ProviderDocumentRequestService.DoesRequestReportForSameYearAlreadyExistAsync(this.requestReportVM.IdCandidateProvider, this.requestReportVM.Year!.Value))
                        {
                            await this.ShowErrorAsync($"Не можете да създадете нов отчет за документи с фабрична номерация за {this.requestReportVM.Year!.Value} г., защото има данни за създаден такъв!");
                            this.loading = false;
                            this.SpinnerHide();
                            return;
                        }
                    }

                    this.validationMessages.Clear();

                    if (this.addedFabricNumbersSource.Any())
                    {
                        this.requestReportVM.DocumentSerialNumbers = this.addedFabricNumbersSource.ToList();
                    }

                    var inputContext = new ResultContext<RequestReportVM>();
                    inputContext.ResultContextObject = this.requestReportVM;
                    var result = new ResultContext<RequestReportVM>();

                    if (this.requestReportVM.IdRequestReport == 0)
                    {
                        result = await this.ProviderDocumentRequestService.CreateRequestReportAsync(inputContext);
                    }
                    else
                    {
                        result = await this.ProviderDocumentRequestService.UpdateRequestReportAsync(inputContext);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.LoadDataForAvailableDocuments();
                        await this.LoadDataForHandedOverDocumentsAsync();
                        await this.LoadDataForPrintedDocumentsAsync();

                        await this.SetCreateAndModifyInfoAsync();

                        this.editContext.MarkAsUnmodified();

                        this.requestReportVM = result.ResultContextObject;

                        await this.CallbackAfterSubmit.InvokeAsync();

                        if (showToastMessage)
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                        }
                    }
                }
                else
                {
                    await this.AddValidationMessagesAsync();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddValidationMessagesAsync()
        {
            this.validationMessages.Clear();
            foreach (var msg in this.editContext.GetValidationMessages())
            {
                if (!this.validationMessages.Contains(msg))
                {
                    this.validationMessages.Add(msg);
                }
            }

            await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
        }

        private async Task AddDocumentSerialNumbersBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var documentSerialNumbers = await this.SetDocumentSerialNumbersData();
                this.SetDocumentSerialNumbersSourceForAddSerialNumbersModal(documentSerialNumbers);

                await this.cpoDocumentDestructionAddSerialNumbersModal.OpenModal(documentSerialNumbers, this.typeOfRequestedDocumentsSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void SetDocumentSerialNumbersSourceForAddSerialNumbersModal(List<DocumentSerialNumberVM> documentSerialNumbers)
        {
            foreach (var docSerialNumber in this.addedFabricNumbersSource)
            {
                documentSerialNumbers.RemoveAll(x => x.SerialNumber == docSerialNumber.SerialNumber);
            }
        }

        private async Task<List<DocumentSerialNumberVM>> SetDocumentSerialNumbersData()
        {
            var documentSerialNumbers = (await this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusReceivedAndYear(new CandidateProviderVM() { IdCandidate_Provider = this.requestReportVM.IdCandidateProvider }, this.requestReportVM.Year.Value)).ToList();
            if (this.addedFabricNumbersSource.Any())
            {
                foreach (var docSerialNumber in this.addedFabricNumbersSource)
                {
                    documentSerialNumbers.RemoveAll(x => x.IdDocumentSerialNumber == docSerialNumber.IdDocumentSerialNumber);
                }
            }

            return documentSerialNumbers.OrderBy(x => x.SerialNumber).ToList();
        }

        private async Task LoadDataForHandedOverDocumentsAsync()
        {
            if (this.requestReportVM.IdRequestReport != 0)
            {
                this.handedOverDocumentsSource = (await this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusSubmittedAndYear(new CandidateProviderVM() { IdCandidate_Provider = this.requestReportVM.IdCandidateProvider }, this.requestReportVM.Year.Value)).OrderBy(x => x.SerialNumber).ToList();
                this.StateHasChanged();
            }
        }

        private async Task LoadDataForPrintedDocumentsAsync()
        {
            if (this.requestReportVM.IdRequestReport != 0)
            {
                this.printedDocumentsSource = (await this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusPrintedAndYear(new CandidateProviderVM() { IdCandidate_Provider = this.requestReportVM.IdCandidateProvider }, this.requestReportVM.Year.Value)).OrderBy(x => x.SerialNumber).ToList();
                this.StateHasChanged();
            }
        }

        private async Task LoadDataForAvailableDocuments()
        {
            if (this.requestReportVM.IdRequestReport != 0)
            {
                this.availableDocumentsSource = await this.SetDocumentSerialNumbersData();
                this.SetDocumentSerialNumbersSourceForAddSerialNumbersModal(this.availableDocumentsSource);

                foreach (var docSerialNumber in this.availableDocumentsSource)
                {
                    var typeOfDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                    if (typeOfDoc is not null)
                    {
                        docSerialNumber.TypeOfRequestedDocument = typeOfDoc;
                    }

                    var docOperation = this.kvActionType.FirstOrDefault(x => x.IdKeyValue == docSerialNumber.IdDocumentOperation);
                    if (docOperation is not null)
                    {
                        docSerialNumber.DocumentOperationName = docOperation.Name;
                    }
                }

                this.StateHasChanged();
            }
        }

        private async Task DeleteDocumentSerialNumber(DocumentSerialNumberVM documentSerialNumber)
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            this.documentSerialNumberToDelete = documentSerialNumber;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                var docSerialNumberFromOperation = await this.ProviderDocumentRequestService.GetDocumentSerialNumberByIdOperationAndIdDocumentSerialNumber(this.documentSerialNumberToDelete.IdDocumentOperation, this.documentSerialNumberToDelete.IdDocumentSerialNumber);
                if (docSerialNumberFromOperation is not null)
                {
                    var inputContext = new ResultContext<DocumentSerialNumberVM>();
                    inputContext.ResultContextObject = this.documentSerialNumberToDelete;
                    var resultContext = await this.ProviderDocumentRequestService.DeleteDocumentSerialNumberAsync(inputContext);

                    if (resultContext.HasErrorMessages)
                    {
                        this.toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await this.toast.sfErrorToast.ShowAsync();
                    }
                    else
                    {
                        var result = await this.ProviderDocumentRequestService.UpdateDocumentCountAfterDocumentSerialNumberDeletionByIdRequestDocumentManagementAsync(this.documentSerialNumberToDelete.IdRequestDocumentManagement);
                        this.toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                        await this.toast.sfSuccessToast.ShowAsync();
                    }
                }
                else
                {
                    this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                    await this.toast.sfSuccessToast.ShowAsync();
                }

                this.addedFabricNumbersSource.RemoveAll(x => x.SerialNumber == this.documentSerialNumberToDelete.SerialNumber);
                await this.addedFabricNumbersGrid.Refresh();
            }
        }

        private async Task UpdateAfterModalSubmit(CPODocumentDestructionAddSerialNumbersReturnInfo cpoDocumentDestructionAddSerialNumbersReturnInfo)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                foreach (var docSerialNumber in cpoDocumentDestructionAddSerialNumbersReturnInfo.DocumentSerialNumbers)
                {
                    var typeOfDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                    if (typeOfDoc is not null)
                    {
                        docSerialNumber.TypeOfRequestedDocument = typeOfDoc;
                    }

                    if (cpoDocumentDestructionAddSerialNumbersReturnInfo.OperationType == "Анулиран")
                    {
                        docSerialNumber.IdDocumentOperation = this.kvCancelled.IdKeyValue;
                        docSerialNumber.DocumentDate = cpoDocumentDestructionAddSerialNumbersReturnInfo.DestructionDate.Value;
                    }
                    else
                    {
                        docSerialNumber.IdDocumentOperation = this.kvDestroyed.IdKeyValue;
                    }

                    var docOperation = this.kvActionType.FirstOrDefault(x => x.IdKeyValue == docSerialNumber.IdDocumentOperation);
                    if (docOperation is not null)
                    {
                        docSerialNumber.DocumentOperationName = docOperation.Name;
                    }
                }

                this.addedFabricNumbersSource.AddRange(cpoDocumentDestructionAddSerialNumbersReturnInfo.DocumentSerialNumbers.ToList());
                await this.addedFabricNumbersGrid.Refresh();

                await this.LoadDataForAvailableDocuments();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task FileInForDestruction()
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            if (!this.addedFabricNumbersSource.Any())
            {
                await this.ShowErrorAsync("Моля, добавете фабричен номер/фабрични номера преди да подадете заявката!");
                return;
            }

            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateReportYear;
            if (this.editContext.Validate())
            {
                var isPeriodValid = await this.IsYearInputValidAsync() && await this.IsDestructionDateValidAsync();
                if (!isPeriodValid)
                {
                    return;
                }
            }
            else
            {
                await this.AddValidationMessagesAsync();
                return;
            }

            string msg = "Сигурни ли сте, че искате да подадете заявката за унищожаване на документи към НАПОО?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                await this.SaveBtn(false);

                if (!this.editContext.GetValidationMessages().Any())
                {
                    var inputContext = new ResultContext<RequestReportVM>();
                    inputContext.ResultContextObject = this.requestReportVM;

                    var result = await this.ProviderDocumentRequestService.FileInProviderRequestReportAsync(inputContext);

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
            }
        }

        private async Task<bool> IsYearInputValidAsync()
        {
            var date = DateTime.Now.Day;
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var time = new DateTime(year, month, date);
            var period = this.reportPeriod;
            var setting = DateTime.Parse(period);
            var result = DateTime.Compare(time, setting);
            if (result < 0)
            {
                var allowedYear = year - 1;
                if (this.requestReportVM.Year != allowedYear)
                {
                    var allowedStartPeriod = DateTime.Parse(this.reportPeriod);
                    allowedStartPeriod.AddDays(1);
                    await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {this.requestReportVM.Year} година към НАПОО само в периода {allowedStartPeriod.ToString("dd.MM")}-31.12!");
                    return false;
                }
            }
            else if (result > 0)
            {
                var allowedYear = year;
                if (this.requestReportVM.Year != allowedYear)
                {
                    await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {this.requestReportVM.Year} година към НАПОО само в периода 01.01-{setting.ToString("dd.MM")}!");
                    return false;
                }
            }
            else if (result == 0)
            {
                var allowedYear = year - 1;
                if (this.requestReportVM.Year != allowedYear)
                {
                    var allowedStartPeriod = DateTime.Parse(this.reportPeriod);
                    allowedStartPeriod.AddDays(1);
                    await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {this.requestReportVM.Year} година към НАПОО само в периода {allowedStartPeriod.ToString("dd.MM")}-31.12!");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> IsDestructionDateValidAsync()
        {
            var date = DateTime.Now.Day;
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var time = new DateTime(year, month, date);
            var period = this.reportPeriod;
            var setting = DateTime.Parse(period);
            var result = DateTime.Compare(time, setting);
            //var destructionYear = int.Parse(this.requestReportVM.DestructionDate.Value.ToString("yyyy"));
            var destructionYearV = this.requestReportVM.DestructionDate.Value.ToString("dd.MM.yyyy");
            var destructionYear = DateTime.Parse(destructionYearV);
            //if (result < 0)
            //{
            //    var allowedYear = year - 1;
            //    if (destructionYear != allowedYear)
            //    {
            //        var allowedStartPeriod = DateTime.Parse(this.reportPeriod);
            //        allowedStartPeriod.AddDays(1);
            //        await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {destructionYear} година към НАПОО само в периода {allowedStartPeriod.ToString("dd.MM")}-31.12!");
            //        return false;
            //    }
            //}
            //else if (result > 0)
            //{
            //    var allowedYear = year;
            //    if (destructionYear != allowedYear)
            //    {
            //        await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {destructionYear} година към НАПОО само в периода 01.01-{setting.ToString("dd.MM")}!");
            //        return false;
            //    }
            //}
            //else if (result == 0)
            //{
            //    var allowedYear = year - 1;
            //    if (destructionYear != allowedYear)
            //    {
            //        var allowedStartPeriod = DateTime.Parse(this.reportPeriod);
            //        allowedStartPeriod.AddDays(1);
            //        await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {destructionYear} година към НАПОО само в периода {allowedStartPeriod.ToString("dd.MM")}-31.12!");
            //        return false;
            //    }
            //}


            if (this.requestReportVM.Year.Value == DateTime.Now.Year && destructionYear < setting)
            {
                var allowedStartPeriod = DateTime.Parse(this.reportPeriod);
                allowedStartPeriod.AddDays(1);
                await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {this.requestReportVM.Year.Value} година към НАПОО само в периода {allowedStartPeriod.ToString("dd.MM")}-31.12!");
                return false;
            }

            if (this.requestReportVM.Year.Value == DateTime.Now.Year - 1 && destructionYear > setting)
            {
                await this.ShowErrorAsync($"Можете да подавате отчети за документи с фабрична номерация за {destructionYear} година към НАПОО само в периода 01.01-{setting.ToString("dd.MM")}!");
                return false;
            }

            return true;
        }

        private async Task PrintProtocol()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var result = this.ProviderDocumentRequestService.PrintProtocol(this.requestReportVM, this.typeOfRequestedDocumentsSource);
                await FileUtils.SaveAs(this.JsRuntime, "Protokol_za_unishtojavane.docx", result.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task PrintReport()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                var result = await this.ProviderDocumentRequestService.PrintReportAsync(this.requestReportVM, this.typeOfRequestedDocumentsSource);
                await FileUtils.SaveAs(this.JsRuntime, "Otchet_za_unishtojavane.docx", result.ToArray());
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
        }

        private async Task AddNewDocumentBtn()
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.cpoDocumentDestructionDocumentModal.OpenModal(new ReportUploadedDocVM() { IdRequestReport = this.requestReportVM.IdRequestReport, IdCandidateProvider = this.requestReportVM.IdCandidateProvider }, this.kvDocumentTypeSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteDocument(ReportUploadedDocVM reportUploadedDocVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            this.documentToDelete = reportUploadedDocVM;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {

                var resultContext = await this.ProviderDocumentRequestService.DeleteReportUploadedDocumentAsync(this.documentToDelete);

                if (resultContext.HasErrorMessages)
                {
                    this.toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await this.toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    this.toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                    await this.toast.sfSuccessToast.ShowAsync();

                    this.reportUploadedDocSource = (await this.ProviderDocumentRequestService.GetAllReportUploadedDocumentsByRequestReportIdAsync(this.requestReportVM.IdRequestReport)).ToList();

                    foreach (var document in this.reportUploadedDocSource)
                    {
                        document.TypeReportUploadedDocumentName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdTypeReportUploadedDocument).Name;
                        document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnDocumentModalSubmit(ReportUploadedDocVM reportUploadedDocVM)
        {
            this.reportUploadedDocSource = (await this.ProviderDocumentRequestService.GetAllReportUploadedDocumentsByRequestReportIdAsync(reportUploadedDocVM.IdRequestReport)).ToList();
            foreach (var document in this.reportUploadedDocSource)
            {
                document.TypeReportUploadedDocumentName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdTypeReportUploadedDocument).Name;
                document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                var typeOfDocument = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdTypeReportUploadedDocument);
                if (typeOfDocument is not null)
                {
                    document.TypeReportUploadedDocumentName = typeOfDocument.Name;
                }
            }

            this.StateHasChanged();
        }

        private async Task OnDownloadClick(string fileName)
        {
            bool hasPermission = await CheckUserActionPermission("ViewCPODocumentDestructionData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                ReportUploadedDocVM document = this.reportUploadedDocSource.FirstOrDefault(x => x.FileName == fileName);

                if (document != null)
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ReportUploadedDoc>(document.IdReportUploadedDoc);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ReportUploadedDoc>(document.IdReportUploadedDoc);

                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileName, documentStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

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

        private void ValidateReportYear(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var allowedYears = new List<int>() { DateTime.Now.Year, DateTime.Now.Year - 1 };
            if (this.requestReportVM.Year.HasValue)
            {
                if (!int.TryParse(this.requestReportVM.Year.Value.ToString(), out int result))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestReportVM, "Year");
                    this.messageStore?.Add(fi, "Полето 'Година' може да бъде само цяло число!");
                    return;
                }

                if (this.requestReportVM.Year.ToString().Length != 4)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestReportVM, "Year");
                    this.messageStore?.Add(fi, "Полето 'Година' не може да съдържа по-малко или повече от 4 числа!");
                    return;
                }

                var yearInput = allowedYears.FirstOrDefault(x => x == this.requestReportVM.Year.Value);
                if (yearInput == 0)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestReportVM, "Year");
                    var firstYear = allowedYears[0];
                    var secondYear = allowedYears[1];
                    this.messageStore?.Add(fi, $"В полето 'Година' може да въведете {firstYear} или {secondYear} година!");
                }
            }

            if (this.requestReportVM.DestructionDate.HasValue)
            {
                var yearInput = allowedYears.FirstOrDefault(x => x == int.Parse(this.requestReportVM.DestructionDate.Value.ToString("yyyy")));
                if (yearInput == 0)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.requestReportVM, "DestructionDate");
                    var firstYear = allowedYears[0];
                    var secondYear = allowedYears[1];
                    this.messageStore?.Add(fi, $"В полето 'Дата на отчета' може да въведете {firstYear} или {secondYear} година!");
                }
            }
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.requestReportVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestReportVM.IdModifyUser);
            this.requestReportVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.requestReportVM.IdCreateUser);
        }
    }
}
