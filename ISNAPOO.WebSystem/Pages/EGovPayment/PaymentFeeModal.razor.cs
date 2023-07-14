using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.EGovPayment
{
    public partial class PaymentFeeModal : BlazorBaseComponent
    {
        private SfDialog sfDialog;
        private PaymentVM paymentVM;
        private int selectedTab = 0;
        private ValidationMessageStore? validationMessageStore;
        private CandidateProvider candidateProvider;
       // private IEnumerable<KeyValueVM> applicantUinType;       
        private KeyValueVM expDay, licenceCPO, licenceCIPOO, licensingFee, applicantUinType, kvPending;
        private IEnumerable<ProcedurePriceVM> procedurePrices;
        private ProcedurePriceVM procedurePrice = new ProcedurePriceVM();
        private int oldIdPaymentStatus = 0;
        private bool procedureSelected = false;
        private bool isDisabled = false;
        private int kvStatus = 0;      
        private List<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();
        private List<string> validationMessages = new List<string>();

        public override bool IsContextModified => this.editContext.IsModified();

        [Inject]
        public IPaymentService PaymentService { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }
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

    
        public async Task OpenPayModal(PaymentVM payment,string obligationType, CandidateProviderVM candidateProviderVM)
        {
           
            //selectedTab = 0;
            this.isDisabled = false;
            this.procedurePrices = (await this.PaymentService.GetAllProcedurePrices()).ToList();
            this.licenceCPO = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO"));
            this.licenceCIPOO = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO"));
            this.applicantUinType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ApplicantUinTypePayEGov", "BULSTAT"));
            this.kvPending = (await this.DataSourceService.GetKeyValueByIntCodeAsync("PaymentStatusPayEGov", "pending"));

            this.validationMessages.Clear();
  
            this.paymentVM = payment;

            if (this.licenceCPO.IdKeyValue == candidateProviderVM.IdTypeLicense)
            {
                if (obligationType == "FirstLicenzing")
                {
                    this.licensingFee = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingFee", "FirstLicenzingCPO"));
                    this.procedurePrice = this.procedurePrices.Where(x => x.IdTypeApplication == this.licensingFee.IdKeyValue).FirstOrDefault();
                }
                else if (obligationType == "StartProcedure")
                {
                    this.licensingFee = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingFee", "StartProcedureCPO"));

                    int profCount = await this.PaymentService.GetProfCount(candidateProviderVM);

                    this.procedurePrice = (procedurePrices.Where(p => (p.IdTypeApplication == this.licensingFee.IdKeyValue
                    && p.CountProfessionsFrom <= profCount && !p.CountProfessionsTo.HasValue) || (p.CountProfessionsTo.HasValue && p.IdTypeApplication == this.licensingFee.IdKeyValue
                    && p.CountProfessionsFrom <= profCount && p.CountProfessionsTo >= profCount))).FirstOrDefault();

                }
                else if (obligationType == "IssuingLicense")
                {
                    this.licensingFee = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingFee", "IssuingLicenseCPO"));
                    this.procedurePrice = this.procedurePrices.Where(x => x.IdTypeApplication == this.licensingFee.IdKeyValue).FirstOrDefault();
                }
            }
            else if (this.licenceCIPOO.IdKeyValue == candidateProviderVM.IdTypeLicense)
            {
                if (obligationType == "FirstLicenzing")
                {
                    this.licensingFee = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingFee", "FirstLicenzingCIPO"));
                }
                else if (obligationType == "StartProcedure")
                {
                    this.licensingFee = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingFee", "StartProcedureCIPO"));
                }
                else if (obligationType == "IssuingLicense")
                {
                    this.licensingFee = (await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingFee", "IssuingLicenseCIPO"));
                }
                this.procedurePrice = this.procedurePrices.Where(x => x.IdTypeApplication == this.licensingFee.IdKeyValue).FirstOrDefault();
            }

            if (this.procedurePrice != null)
            {
                this.paymentVM.PaymentReason = this.procedurePrice.Name;
                this.paymentVM.AdditionalInformation = this.procedurePrice.AdditionalInformation;
                this.paymentVM.PaymentAmount = (double)this.procedurePrice.Price;
                this.paymentVM.IdProcedurePrice = this.procedurePrice.IdProcedurePrice;
            }

            this.paymentVM.ObligationType = this.licensingFee.Name;

            this.paymentVM.IdCandidate_Provider = candidateProviderVM.IdCandidate_Provider;
            this.paymentVM.ApplicantUin = candidateProviderVM.PoviderBulstat;
            this.paymentVM.ApplicantName = candidateProviderVM.ProviderOwner;

            this.paymentVM.ApplicantUinTypeId = applicantUinType.IdKeyValue;
            this.paymentVM.ApplicantUinIntDefVal = applicantUinType.DefaultValue1;
            this.paymentVM.IdPaymentStatus = kvPending.IdKeyValue;

          
            this.paymentVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.paymentVM.IdModifyUser);
            this.paymentVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.paymentVM.IdCreateUser);
            this.editContext = new EditContext(this.paymentVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task Submit()
        {
            string msg = "Сигурни ли сте, че искате да направите заявка за плащане към НАПОО на избраната такса за лицензиране?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
              await SendPayment();
            }
        }  
        
        public async Task SendPayment()
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

                    result = await this.PaymentService.CreatePaymentToPayEGov(result);
                    this.paymentVM = result.ResultContextObject;
                    addForConcurrencyCheck = true;

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                        // await this.SetCreateAndModifyInfoAsync();
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
            this.isDisabled = true;
            this.SpinnerHide();
        }
        private void ValidateCode(object? sender, ValidationRequestedEventArgs args)
        {
            this.validationMessageStore?.Clear();

            var paymentReason = this.paymentVM.PaymentReason;

            FieldIdentifier fi = new FieldIdentifier();

            if (string.IsNullOrEmpty(paymentReason))
            {
                fi = new FieldIdentifier(this.paymentVM, "PaymentReason");
                this.validationMessageStore?.Add(fi, "Полето 'Основание за плащане' е задължително!");
            }
        } 
    }
}
