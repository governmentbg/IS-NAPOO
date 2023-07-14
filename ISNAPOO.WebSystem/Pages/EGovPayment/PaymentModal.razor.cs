using System.Security.AccessControl;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Common.HelperClasses;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SkiaSharp;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.SPPOO;
using Data.Models.Data.EGovPayment;
//using static SkiaSharp.HarfBuzz.SKShaper;

namespace ISNAPOO.WebSystem.Pages.EGovPayment
{
    public partial class PaymentModal : BlazorBaseComponent
    {
        private SfDialog sfDialog;
        private PaymentVM paymentVM;
        private int selectedTab = 0;
        private ValidationMessageStore? validationMessageStore;
        private CandidateProvider candidateProvider;
        IEnumerable<KeyValueVM> applicantUinType, paymentStatuses, kvProcedures;
        KeyValueVM expDay;
        IEnumerable<ProcedurePriceVM> procedurePrices;
        private int oldIdPaymentStatus = 0;
        private bool procedureSelected = false;
        int kvPending = 0;
        int kvStatus = 0;
        private SfAutoComplete<int, CandidateProviderVM> sfAutoCompleteCPO = new SfAutoComplete<int, CandidateProviderVM>();

        public List<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();

        private List<string> validationMessages = new List<string>();

        [Inject]
        public IPaymentService PaymentService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        ICandidateProviderService candidateProviderService { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        public override bool IsContextModified => this.editContext.IsModified();


        public async Task openPayModal(PaymentVM payment)
        {

            selectedTab = 0;
            this.applicantUinType = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicantUinTypePayEGov")).ToList();
            this.paymentStatuses = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("PaymentStatusPayEGov")).ToList();
            this.candidateProviders = (await candidateProviderService.GetAllActiveCPOCandidateProvidersWithoutAnythingIncludedAsync()).ToList();
            //this.kvProcedures = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("")).ToList();

            this.procedurePrices = (await this.PaymentService.GetAllProcedurePrices()).ToList();

            this.validationMessages.Clear();
            this.kvPending = this.DataSourceService.GetKeyValueByIntCodeAsync("PaymentStatusPayEGov", "pending").Result.IdKeyValue;
            this.paymentVM = payment;
            oldIdPaymentStatus = this.paymentVM.IdPaymentStatus;
            this.paymentVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.paymentVM.IdModifyUser);
            this.paymentVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.paymentVM.IdCreateUser);
            this.editContext = new EditContext(this.paymentVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.isVisible = true;
            this.StateHasChanged();
        }
        public async Task OnAutoCompleteCPO(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {

                try
                {
                    var query = new Query().Where(new WhereFilter() { Field = "ProviderName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteCPO.FilterAsync(this.candidateProviders, query);
                    
                    
                }
                catch (Exception e) { }

            }
        }
        public async Task Submit()
        {
            this.validationMessages.Clear();
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.editContext = new EditContext(this.paymentVM);

                this.editContext.EnableDataAnnotationsValidation();
                this.validationMessageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateCode;

                if (this.editContext.Validate())
                {
                    this.SpinnerShow();
                    var result = new ResultContext<PaymentVM>();

                    result.ResultContextObject = this.paymentVM;
                    var addForConcurrencyCheck = false;

                    if (this.paymentVM.IdPayment == 0)
                    {
                        result.ResultContextObject.ApplicantUinIntDefVal = (await DataSourceService.GetKeyValueByIdAsync(result.ResultContextObject.ApplicantUinTypeId)).DefaultValue1;
                        result = await this.PaymentService.CreatePaymentToPayEGov(result);
                        this.paymentVM = result.ResultContextObject;
                        addForConcurrencyCheck = true;
                    }
                    else
                    {

                        if (result.ResultContextObject.IdPaymentStatus == oldIdPaymentStatus)
                        {
                            result = await this.PaymentService.UpdatePaymentAsync(result);
                            //за тест
                            //  result = await this.PaymentService.CreatePaymentToPayEGov(result);
                            //за тест
                        }
                        else
                        {
                            result.ResultContextObject.PaymentStatusDefVal = (await DataSourceService.GetKeyValueByIdAsync(result.ResultContextObject.IdPaymentStatus)).DefaultValue1;
                            result = await this.PaymentService.SetStatusPaymentRequest(result);
                        }
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.SetCreateAndModifyInfoAsync();

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }
            this.SpinnerHide();
        }
        private void ValidateCode(object? sender, ValidationRequestedEventArgs args)
        {
            this.validationMessageStore?.Clear();

            var candidateProviderId = this.paymentVM.IdCandidate_Provider;
            var applicantUinTypeId = this.paymentVM.ApplicantUinTypeId;
            var applicantUin = this.paymentVM.ApplicantUin;
            var idPaymentStatus = this.paymentVM.IdPaymentStatus;

            FieldIdentifier fi = new FieldIdentifier();

            if (candidateProviderId == 0)
            {
                fi = new FieldIdentifier(this.paymentVM, "IdCandidate_Provider");
                this.validationMessageStore?.Add(fi, "Полето 'ЦПО' е задължително!");
            }
            if (applicantUinTypeId == 0)
            {
                fi = new FieldIdentifier(this.paymentVM, "ApplicantUinTypeId");
                this.validationMessageStore?.Add(fi, "Полето 'Tип на идентификатора на задължено лице' е задължително!");
            }
            if (applicantUinTypeId == 5410)
            {
                var checkEGN = new BasicEGNValidation(applicantUin);
                if (!checkEGN.Validate())
                {
                    fi = new FieldIdentifier(this.paymentVM, "ApplicantUin");
                    this.validationMessageStore?.Add(fi, "ЕГН-то в полето 'Идентификатор на задължено лице' не е валидно!");
                }
            }
            else if (applicantUinTypeId == 5411)
            {
                var checkLnc = new BasicLncValidation(applicantUin);
                if (!checkLnc.Validate())
                {
                    fi = new FieldIdentifier(this.paymentVM, "ApplicantUin");
                    this.validationMessageStore?.Add(fi, "ЛНЧ-то в полето 'Идентификатор на задължено лице' не е валидно!");
                }
            }
            else if (applicantUinTypeId == 5412)
            {
                fi = new FieldIdentifier(this.paymentVM, "ApplicantUin");
                if (applicantUin.Length == 9)
                {
                    if (!EIKValidator.calculateChecksumForNineDigitsEIK(applicantUin))
                    {
                        this.validationMessageStore?.Add(fi, "БУЛСТАТ-а с 9 цифри в полето 'Идентификатор на задължено лице' не е валидно!");
                    }
                }
                else if (applicantUin.Length == 13)
                {
                    try
                    {
                        if (!EIKValidator.calculateChecksumForThirteenDigitsEIK(applicantUin))
                        {
                            this.validationMessageStore?.Add(fi, "БУЛСТАТ-а с 13 цифри в полето 'Идентификатор на задължено лице' не е валидно!");
                        }
                    }
                    catch (Exception)
                    {
                        this.validationMessageStore?.Add(fi, "БУЛСТАТ-а с 13 цифри в полето 'Идентификатор на задължено лице' не е валидно!");
                    }
                }
                else
                {
                    this.validationMessageStore?.Add(fi, "БУЛСТАТ-а в полето 'Идентификатор на задължено лице' не е валидно!");
                }
            }

            if (idPaymentStatus == 0)
            {
                fi = new FieldIdentifier(this.paymentVM, "IdPaymentStatus");
                this.validationMessageStore?.Add(fi, "Полето 'Статус' е задължително!");
            }
        }
        private async Task SetCreateAndModifyInfoAsync()
        {
            this.paymentVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.paymentVM.IdModifyUser);
            this.paymentVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.paymentVM.IdCreateUser);
        }
        private async Task UpdatePaymentStatusEgov()
        {
            ResultContext<PaymentVM> payment = new ResultContext<PaymentVM>();

            payment.ResultContextObject = this.paymentVM;
            var resultStatus = new ResultContext<PaymentStatus>();

            try
            {
                resultStatus = await this.PaymentService.GetPaymentsStatus(payment);

                //resultPayment = await this.PaymentService.UpdatePaymentStatusAsync(resultStatus);

                if (resultStatus.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, resultStatus.ListErrorMessages));
                }
                else
                {
                    this.kvStatus = this.DataSourceService.GetKeyValueByIntCodeAsync("PaymentStatusPayEGov", resultStatus.ResultContextObject.Status.ToLower()).Result.IdKeyValue;
                    payment.ResultContextObject.IdPaymentStatus = this.kvStatus;

                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultStatus.ListMessages));

                    await this.SetCreateAndModifyInfoAsync();

                    await this.CallbackAfterSubmit.InvokeAsync();

                    this.StateHasChanged();
                }
            }
            catch (Exception)
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
            }

        }
        private async Task OnFormEikValueChanged(ChangeEventArgs<int, KeyValueVM> args)
        {
            if (args.Value != null && paymentVM.ApplicantUin != null)
            {
              
            }            
        }
        private async Task OnFormProcedurePriceValueChanged(ChangeEventArgs<int, ProcedurePriceVM> args)
        {
            if (args.ItemData != null)
            {
                //this.procedureSelected = true;
                this.paymentVM.PaymentAmount = (double?)args.ItemData.Price;
                this.paymentVM.PaymentReason = args.ItemData.Name;
                this.paymentVM.AdditionalInformation = args.ItemData.AdditionalInformation;
                int idTypeApp = args.ItemData.IdTypeApplication;
                this.expDay = (await this.DataSourceService.GetKeyValueByIdAsync(idTypeApp));
                var expDayDefValue = expDay.DefaultValue1;
                var newDate = DateTime.Now;
                this.paymentVM.ExpirationDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23, 59, 59).AddDays(int.Parse(expDayDefValue));
            }
            else
            {
                this.paymentVM.PaymentAmount = 0;
                this.paymentVM.PaymentReason = null;
                this.paymentVM.AdditionalInformation = null;
            }
        }
        private async Task OnFormCandProviderCompleteData(ChangeEventArgs<int, CandidateProviderVM> args)
        {
            if (args.ItemData != null)
            {
                
                this.paymentVM.ApplicantName = args.ItemData.ProviderOwner;
                this.paymentVM.ApplicantUin = args.ItemData.PoviderBulstat;
                KeyValueVM appUinType = await this.DataSourceService.GetKeyValueByIntCodeAsync("ApplicantUinTypePayEGov", "BULSTAT");
                this.paymentVM.ApplicantUinTypeId = appUinType.IdKeyValue;
            }
            else
            {
                this.paymentVM.ApplicantName = null;
                this.paymentVM.ApplicantUin = null;
                this.paymentVM.ApplicantUinTypeId = 0;
            }
        }
    }
}
