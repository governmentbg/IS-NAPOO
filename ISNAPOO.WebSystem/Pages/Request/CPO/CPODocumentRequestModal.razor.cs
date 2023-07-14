using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.EKATTE;
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
using Data.Models.Data.Request;
using Data.Models.Data.Common;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPODocumentRequestModal : BlazorBaseComponent
    {
        ToastMsg toast = new ToastMsg();
        private SfDialog sfDialog = new SfDialog();
        private SfGrid<TypeOfRequestedDocumentVM> addedDocumentsGrid = new SfGrid<TypeOfRequestedDocumentVM>();
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocationCorrespondence = new SfAutoComplete<int?, LocationVM>();

        private List<string> validationMessages = new List<string>();
        private ProviderRequestDocumentVM providerDocumentRequestVM = new ProviderRequestDocumentVM();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentSource = new List<TypeOfRequestedDocumentVM>();
        private List<TypeOfRequestedDocumentVM> addedDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private RequestDocumentTypeVM requestDocumentTypeVM = new RequestDocumentTypeVM();
        private TypeOfRequestedDocumentVM documentToDelete = new TypeOfRequestedDocumentVM();
        private CandidateProviderVM providerVM = new CandidateProviderVM();
        private List<LocationVM> locationsSource = new List<LocationVM>();
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        private string documentRequestPeriod = string.Empty;
        private ValidationMessageStore? messageStore;

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSave { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.providerDocumentRequestVM);
        }

        public async Task OpenModal(ProviderRequestDocumentVM providerRequestDocumentVM, CandidateProviderVM providerVM)
        {
            this.editContext = new EditContext(this.providerDocumentRequestVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.requestDocumentTypeVM = new RequestDocumentTypeVM();

            this.typeOfRequestedDocumentSource = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();
            this.validationMessages.Clear();

            if (providerRequestDocumentVM.IdLocationCorrespondence != null)
            {
                LocationVM locationCorrespondence = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(providerRequestDocumentVM.IdLocationCorrespondence ?? default);
                this.locationsSource.Add(locationCorrespondence);
            }

            this.providerDocumentRequestVM = providerRequestDocumentVM;
            this.providerVM = providerVM;
            if (this.providerDocumentRequestVM.IdProviderRequestDocument == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                this.providerDocumentRequestVM.CreatePersonName = "";
                this.providerDocumentRequestVM.ModifyPersonName = "";
            }
            else
            {
                this.CreationDateStr = this.providerDocumentRequestVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.providerDocumentRequestVM.ModifyDate.ToString("dd.MM.yyyy");
                this.providerDocumentRequestVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.providerDocumentRequestVM.IdModifyUser);
                this.providerDocumentRequestVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.providerDocumentRequestVM.IdCreateUser);
            }
            await this.SetAddedDocumentsSourceData(true);
            this.SetTypeOfRequestedDocumentSource();

            this.documentRequestPeriod = (await this.DataSourceService.GetSettingByIntCodeAsync("RequestDocumentPeriod")).SettingValue;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SetAddedDocumentsSourceData(bool clearAddedDocuments)
        {
            if (clearAddedDocuments)
            {
                this.addedDocumentsSource.Clear();
            }

            if (this.providerDocumentRequestVM is not null)
            {
                if (this.providerDocumentRequestVM.RequestDocumentTypes.Any())
                {
                    foreach (var type in this.providerDocumentRequestVM.RequestDocumentTypes)
                    {
                        this.addedDocumentsSource.RemoveAll(x => x.IdTypeOfRequestedDocument == type.IdTypeOfRequestedDocument);
                    }

                    var listIds = this.providerDocumentRequestVM.RequestDocumentTypes.Select(x => x.IdTypeOfRequestedDocument).ToList();
                    var requestedDocuments = await this.ProviderDocumentRequestService.GetTypesOfRequestedDocumentByListIdsAsync(listIds);

                    foreach (var doc in requestedDocuments)
                    {
                        var typeOfDoc = this.typeOfRequestedDocumentSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == doc.IdTypeOfRequestedDocument);
                        typeOfDoc.Quantity = this.providerDocumentRequestVM.RequestDocumentTypes.FirstOrDefault(x => x.IdTypeOfRequestedDocument == typeOfDoc.IdTypeOfRequestedDocument).DocumentCount;
                        this.addedDocumentsSource.Add(typeOfDoc);
                    }
                }
            }

            this.addedDocumentsSource = this.addedDocumentsSource.OrderBy(x => x.Order).ToList();
        }

        private void SetTypeOfRequestedDocumentSource()
        {
            if (this.providerDocumentRequestVM is not null)
            {
                if (this.providerDocumentRequestVM.RequestDocumentTypes.Any())
                {
                    foreach (var type in this.providerDocumentRequestVM.RequestDocumentTypes)
                    {
                        this.typeOfRequestedDocumentSource.RemoveAll(x => x.IdTypeOfRequestedDocument == type.IdTypeOfRequestedDocument);
                    }
                }
            }
        }

        private async Task FileInRequest()
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentRequestData", false);
            if (!hasPermission) { return; }

            string msg = "Сигурни ли сте, че искате да подадете заявката за документация към НАПОО? След подаване на заявката към НАПОО, няма да може да извършвате промени в попълнената информация.";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                this.SpinnerShow();

                if (!this.addedDocumentsSource.Any())
                {
                    await this.ShowErrorAsync("Моля, добавете поне един вид на документ, преди да подадете заявката към НАПОО!");
                    this.SpinnerHide();
                    return;
                }

                var isRequestYearValid = await this.IsRequestYearValidAsync();
                if (!isRequestYearValid)
                {
                    this.SpinnerHide();
                    return;
                }
                    await this.Save(false);

                    if (!this.validationMessages.Any())
                    {
                        var inputContext = new ResultContext<ProviderRequestDocumentVM>();
                        inputContext.ResultContextObject = this.providerDocumentRequestVM;

                        var result = await this.ProviderDocumentRequestService.FileInProviderRequestDocumentAsync(inputContext);

                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                            await this.CallbackAfterSave.InvokeAsync();
                        }

                    }
                

                this.SpinnerHide();
            }
        }

        private async Task<bool> IsRequestYearValidAsync()
        {
            var date = DateTime.Now.Day;
            var month = DateTime.Now.Month;
            var year = DateTime.Now.Year;
            var time = new DateTime(year, month, date);
            var period = this.documentRequestPeriod;
            var setting = DateTime.Parse(period);
            var result = DateTime.Compare(time, setting);
            if (result < 0)
            {
                var allowedYear = year - 1;
                if (this.providerDocumentRequestVM.CurrentYear != allowedYear)
                {
                    var allowedStartPeriod = DateTime.Parse(this.documentRequestPeriod);
                    allowedStartPeriod.AddDays(1);
                    await this.ShowErrorAsync($"Можете да подавате заявки за документация за {this.providerDocumentRequestVM.CurrentYear} година към НАПОО само в периода {allowedStartPeriod.ToString("dd.MM")}-31.12!");
                    return false;
                }
            }
            else if (result > 0)
            {
                var allowedYear = year;
                if (this.providerDocumentRequestVM.CurrentYear != allowedYear)
                {
                    await this.ShowErrorAsync($"Можете да подавате заявки за документация за {this.providerDocumentRequestVM.CurrentYear} година към НАПОО само в периода 01.01-{setting.ToString("dd.MM")}!");
                    return false;
                }
            }
            else if (result == 0)
            {
                var allowedYear = year - 1;
                if (this.providerDocumentRequestVM.CurrentYear != allowedYear)
                {
                    var allowedStartPeriod = DateTime.Parse(this.documentRequestPeriod);
                    allowedStartPeriod.AddDays(1);
                    await this.ShowErrorAsync($"Можете да подавате заявки за документация за {this.providerDocumentRequestVM.CurrentYear} година към НАПОО само в периода {allowedStartPeriod.ToString("dd.MM")}-31.12!");
                    return false;
                }
            }

            return true;
        }

        private async Task Save(bool showToastMessage)
        {
            bool hasPermission = await CheckUserActionPermission("ManageCPODocumentRequestData", false);
            if (!hasPermission) { return; }
            this.SpinnerShow();
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.validationMessages.Clear();
                this.editContext = new EditContext(this.providerDocumentRequestVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateCurrentYearInput;

                if (this.editContext.Validate())
                {
                    foreach (var entry in this.addedDocumentsSource)
                    {
                        if (!this.providerDocumentRequestVM.RequestDocumentTypes.Any(x => x.IdTypeOfRequestedDocument == entry.IdTypeOfRequestedDocument))
                        {
                            RequestDocumentTypeVM requestDocumentTypeVM = new RequestDocumentTypeVM()
                            {
                                IdCandidateProvider = this.providerVM.IdCandidate_Provider,
                                IdTypeOfRequestedDocument = entry.IdTypeOfRequestedDocument,
                                DocumentCount = entry.Quantity
                            };

                            this.providerDocumentRequestVM.RequestDocumentTypes.Add(requestDocumentTypeVM);
                        }
                    }

                    ResultContext<ProviderRequestDocumentVM> resultContext = new ResultContext<ProviderRequestDocumentVM>();
                    resultContext.ResultContextObject = this.providerDocumentRequestVM;

                    var result = await this.ProviderDocumentRequestService.CreateProviderRequestDocumentAsync(resultContext);
                    this.CreationDateStr = result.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
                    this.ModifyDateStr = result.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                    this.providerDocumentRequestVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(result.ResultContextObject.IdModifyUser);
                    this.providerDocumentRequestVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(result.ResultContextObject.IdCreateUser);

                    if (showToastMessage)
                    {
                        if (result.ListMessages.Any())
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                            await this.CallbackAfterSave.InvokeAsync();
                        }
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                }
                else
                {
                    this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnAddDocumentClickHandler()
        {
            if (this.requestDocumentTypeVM.IdTypeOfRequestedDocument == 0)
            {
                this.toast.sfErrorToast.Content = "Моля, изберете вид на документ!";
                await this.toast.sfErrorToast.ShowAsync();
            }
            else if (this.requestDocumentTypeVM.DocumentCount == 0)
            {
                this.toast.sfErrorToast.Content = "Моля, въведете количество на избрания документ!";
                await this.toast.sfErrorToast.ShowAsync();
            }
            else if (this.requestDocumentTypeVM.DocumentCount < 0)
            {
                this.toast.sfErrorToast.Content = "Моля, въведете положителна стойност за количество на избрания документ!";
                await this.toast.sfErrorToast.ShowAsync();
            }
            else
            {
                var typeOfReqDocument = await this.ProviderDocumentRequestService.GetTypeOfRequestedDocumentByIdAsync(new TypeOfRequestedDocumentVM() { IdTypeOfRequestedDocument = this.requestDocumentTypeVM.IdTypeOfRequestedDocument });

                if (typeOfReqDocument is not null)
                {
                    if (!this.addedDocumentsSource.Any(x => x.IdTypeOfRequestedDocument == typeOfReqDocument.IdTypeOfRequestedDocument))
                    {
                        typeOfReqDocument.Quantity = this.requestDocumentTypeVM.DocumentCount;
                        this.addedDocumentsSource.Add(typeOfReqDocument);
                        this.requestDocumentTypeVM.DocumentCount = 0;
                        this.typeOfRequestedDocumentSource = this.typeOfRequestedDocumentSource.Where(x => x.IdTypeOfRequestedDocument != typeOfReqDocument.IdTypeOfRequestedDocument).ToList();

                        this.addedDocumentsSource = this.addedDocumentsSource.OrderBy(x => x.Order).ToList();
                        await this.addedDocumentsGrid.Refresh();

                        this.requestDocumentTypeVM.IdTypeOfRequestedDocument = 0;
                    }
                }
            }
        }

        private async Task DeleteDocument(TypeOfRequestedDocumentVM typeOfRequestedDocumentVM)
        {
            this.documentToDelete = typeOfRequestedDocumentVM;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {

                var requestDocumentType = await this.ProviderDocumentRequestService.GetRequestDocumentTypeByIdTypeOfRequestDocumentAndIdProviderRequestDocumentAsync(new RequestDocumentTypeVM() { IdTypeOfRequestedDocument = this.documentToDelete.IdTypeOfRequestedDocument, IdProviderRequestDocument = this.providerDocumentRequestVM.IdProviderRequestDocument });
                if (requestDocumentType is not null)
                {
                    var resultContext = await this.ProviderDocumentRequestService.DeleteRequestDocumentTypeByIdAsync(requestDocumentType);

                    if (resultContext.HasErrorMessages)
                    {
                        this.toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                        await this.toast.sfErrorToast.ShowAsync();
                    }
                    else
                    {
                        this.providerDocumentRequestVM = await this.ProviderDocumentRequestService.GetProviderRequestDocumentByIdAsync(this.providerDocumentRequestVM);
                        this.providerDocumentRequestVM.RequestStatus = "Създадена";

                        this.toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                        await this.toast.sfSuccessToast.ShowAsync();

                        this.addedDocumentsSource.RemoveAll(x => x.IdTypeOfRequestedDocument == this.documentToDelete.IdTypeOfRequestedDocument);
                    }
                }
                else
                {
                    this.addedDocumentsSource.RemoveAll(x => x.IdTypeOfRequestedDocument == this.documentToDelete.IdTypeOfRequestedDocument);
                    this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                    await this.toast.sfSuccessToast.ShowAsync();
                }

                this.typeOfRequestedDocumentSource = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();
                await this.SetAddedDocumentsSourceData(false);
                this.SetTypeOfRequestedDocumentSource();
                await this.addedDocumentsGrid.Refresh();
            }
        }

        private async Task PrintRequestDocument()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var isDocumentUploaded = !string.IsNullOrEmpty(this.providerDocumentRequestVM.UploadedFileName);

                if (isDocumentUploaded)
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ProviderRequestDocument>(this.providerDocumentRequestVM.IdProviderRequestDocument);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ProviderRequestDocument>(this.providerDocumentRequestVM.IdProviderRequestDocument);

                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.providerDocumentRequestVM.FileName, documentStream.MS!.ToArray());
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
            this.typeOfRequestedDocumentSource = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();
            var result = await this.ProviderDocumentRequestService.PrintRequestDocumentAsync(this.providerDocumentRequestVM, this.typeOfRequestedDocumentSource, this.providerVM);
            await FileUtils.SaveAs(this.JsRuntime, "Zaqvka_dokumentaciq_CPO.pdf", result.ToArray());

            var resultFromUpload = await this.UploadFileService.UploadFileAsync<ProviderRequestDocument>(result, "Zaqvka_dokumentaciq_CPO.pdf", "ProviderRequestDocument", this.providerDocumentRequestVM.IdProviderRequestDocument);
            if (!string.IsNullOrEmpty(resultFromUpload))
            {
                this.providerDocumentRequestVM.UploadedFileName = resultFromUpload;
            }

            await this.ProviderDocumentRequestService.UpdateProviderRequestDocumentUploadedFileNameAsync(this.providerDocumentRequestVM.IdProviderRequestDocument, this.providerDocumentRequestVM.UploadedFileName);
        }

        private async Task OnFilterLocation(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.locationsSource = (List<LocationVM>)await this.LocationService.GetAllLocationsByKatiAsync(args.Text);
                }
                catch (Exception ex) { }

                var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.sfAutoCompleteLocationCorrespondence.FilterAsync(this.locationsSource, query);
            }
        }

        private void ValidateCurrentYearInput(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var allowedYears = new List<int>() { DateTime.Now.Year, DateTime.Now.Year - 1 };

            if (this.providerDocumentRequestVM.CurrentYear.HasValue)
            {
                if (!int.TryParse(this.providerDocumentRequestVM.CurrentYear.Value.ToString(), out int result))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.providerDocumentRequestVM, "CurrentYear");
                    this.messageStore?.Add(fi, "Полето 'Година' може да бъде само цяло число!");
                    return;
                }

                if (this.providerDocumentRequestVM.CurrentYear.ToString().Length != 4)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.providerDocumentRequestVM, "CurrentYear");
                    this.messageStore?.Add(fi, "Полето 'Година' не може да съдържа по-малко или повече от 4 числа!");
                    return;
                }

                var yearInput = allowedYears.FirstOrDefault(x => x == this.providerDocumentRequestVM.CurrentYear.Value);
                if (yearInput == 0)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.providerDocumentRequestVM, "CurrentYear");
                    var firstYear = allowedYears[0];
                    var secondYear = allowedYears[1];
                    this.messageStore?.Add(fi, $"В полето 'Година' за заявките за документация може да въведете {firstYear} или {secondYear} година!");
                }
            }
        }
    }
}
