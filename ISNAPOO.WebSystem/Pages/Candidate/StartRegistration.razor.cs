using System.Text.RegularExpressions;
using System.Timers;
using Data.Models;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using RegiX.Class.AVTR.GetActualState;
using Syncfusion.Blazor.Inputs;
using Timer = System.Timers.Timer;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class StartRegistration : BlazorBaseComponent
    {


        IEnumerable<KeyValueVM> licensingType;
        IEnumerable<KeyValueVM> typeApplication;

        private StartRegistrationModel startRegistrationModel = new StartRegistrationModel()
        {
            EAuthStatusName = "",
            EAuthPersonName = "",
            EAuthEmail = "",//"Zhivko_Hristov@abv.bg",
            EAuthEGN = "",//1212124563, 6001104736
            EIK = "",
            IdLicensingType = 0,
            Email = ""
        };

        private Timer timer;

        bool useAttorney = false;

        bool isGetDataFromRegix = false;

        int IdCandidate_Provider = 0;

        List<KeyValueVM> managers;

        ValidationMessageStore? messageStore;

        SfTextBox tbxAttorneyName = new SfTextBox();
        SfTextBox tbxManagerName = new SfTextBox();

        ActualStateResponseType actualStateResponseType = new ActualStateResponseType();
        StateOfPlay stateOfPlay = new StateOfPlay();

        [Inject]
        public NavigationManager navMgr { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.startRegistrationModel);
            this.isVisible = true;
            managers = new List<KeyValueVM>();

            this.licensingType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType");
            this.typeApplication = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");


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
                    startRegistrationModel.EAuthEGN = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "EGN").Value.ToString();
                    startRegistrationModel.EAuthStatusName = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "ResponseStatus").Value.ToString();
                    startRegistrationModel.EAuthPersonName = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "LatinNames").Value.ToString();
                    startRegistrationModel.EAuthEmail = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "Email").Value.ToString();

                }
                else
                {
                    this.ShowErrorAsync("Опитвате се да заредите линк с изтекла валидност.");
                    isSubmitClicked = true;
                }

                this.StateHasChanged();
            }



        }

        private async Task CheckEIKinReGIX()
        {
            var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);
            this.actualStateResponseType = RegiXService.GetActualState(startRegistrationModel.EIK, callContext);
            if (this.actualStateResponseType == null)
            {
                this.stateOfPlay = RegiXService.GetStateOfPlay(startRegistrationModel.EIK, "", "", 0, RegiXService.GetCallContext());
            }

            await this.LogRegiXRequestAsync(callContext, actualStateResponseType != null);
            this.isGetDataFromRegix = true;

            this.startRegistrationModel.Email = this.startRegistrationModel.EAuthEmail;


            this.startRegistrationModel.CompanyName = this.actualStateResponseType.Company + " " + actualStateResponseType.LegalForm.LegalFormAbbr;


            managers = this.actualStateResponseType.Details.Where(d => d.FieldCode == "10").Select(m => new KeyValueVM() { Name = m.Subject.Name, KeyValueIntCode = m.Subject.Indent }).ToList();


            if (managers.Any(c => c.KeyValueIntCode == this.startRegistrationModel.EAuthEGN))
            {
                this.startRegistrationModel.ManagerName = managers.FirstOrDefault(c => c.KeyValueIntCode == this.startRegistrationModel.EAuthEGN).Name;
                this.startRegistrationModel.Indent = this.startRegistrationModel.EAuthEGN;
            }
            else
            {
                useAttorney = true;
                var callContextEGN = await this.GetCallContextAsync(this.IndividualCheckKV);
                var validPerson = RegiXService.ValidPersonResponseType(this.startRegistrationModel.EAuthEGN, callContextEGN);
                await this.LogRegiXRequestAsync(callContextEGN, validPerson != null);
                this.startRegistrationModel.AttorneyName = $"{validPerson.FirstName} {validPerson.SurName} {validPerson.FamilyName}";
                this.startRegistrationModel.Indent = this.startRegistrationModel.EAuthEGN;

            }


            this.StateHasChanged();


        }

        private void ValidateManagerNameValue(object? sender, ValidationRequestedEventArgs args)
        {
            
            if (!string.IsNullOrEmpty(this.startRegistrationModel.ManagerName))
            {
                //if (!int.TryParse(this.applicationListFilterVM.UIN.Trim(), out int uinValue))
                //{
                //    FieldIdentifier fi = new FieldIdentifier(this.startRegistrationModel, "UIN");
                //    this.messageStore?.Add(fi, "Полето 'Представлявано от' може да съдържа само текст на български език!");
                //}
            }
        }


        private async Task StartRegistrationProcedure()
        {

            if (this.isSubmitClicked == true) { return; }

            this.editContext = new EditContext(this.startRegistrationModel);
            this.editContext.OnValidationRequested += ValidationRequested;
            this.messageStore = new ValidationMessageStore(this.editContext);
            //this.editContext.OnValidationRequested += this.ValidateManagerNameValue;
            this.editContext.EnableDataAnnotationsValidation();

            bool isValid = this.editContext.Validate();

            if (isValid)
            {
                this.isSubmitClicked = true;

                ResultContext<CandidateProviderVM> currentContext = new ResultContext<CandidateProviderVM>();

                CandidateProviderVM candidateProviderVM = new CandidateProviderVM();

                candidateProviderVM.ProviderOwner = this.actualStateResponseType.Company + " " + actualStateResponseType.LegalForm.LegalFormAbbr;
                candidateProviderVM.PoviderBulstat = this.actualStateResponseType.UIC;
                candidateProviderVM.ManagerName = this.startRegistrationModel.ManagerName;
                candidateProviderVM.AttorneyName = this.startRegistrationModel.AttorneyName;
                candidateProviderVM.Indent = this.startRegistrationModel.EAuthEGN;

                if (this.isGetDataFromRegix)
                {
                    candidateProviderVM.IdProviderRegistration = this.DataSourceService.GetKeyValueByIntCodeAsync("ProviderRegisteredAt", "CommercialRegister").Result.IdKeyValue;
                }



                var SeatLocation = await this.LocationService.GetAllLocationsAsync(new LocationVM() { LocationCode = this.actualStateResponseType.Seat.Address.SettlementEKATTE });
                candidateProviderVM.IdLocation = SeatLocation?.FirstOrDefault().idLocation;
                candidateProviderVM.Location = null;
                candidateProviderVM.LocationCorrespondence = null;

                //гр. София 1113, р-н Изгрев, жк. Изток, бл. 4, вх. А, ет. 1, ап. 2
                candidateProviderVM.ProviderAddress = this.RegiXService.GetFormattedAddress(this.actualStateResponseType.Seat.Address);
                candidateProviderVM.ZipCode = this.actualStateResponseType.Seat.Address.PostCode;
                candidateProviderVM.ProviderName = candidateProviderVM.ProviderOwner;
                candidateProviderVM.IdTypeLicense = this.startRegistrationModel.IdLicensingType;
                candidateProviderVM.ProviderFax = this.actualStateResponseType.Seat.Contacts.Fax;
                candidateProviderVM.ProviderWeb = this.actualStateResponseType.Seat.Contacts.URL;
                candidateProviderVM.ProviderEmail = this.startRegistrationModel.Email;
                candidateProviderVM.Title = this.startRegistrationModel.Title;

                var licensingCPO = this.licensingType.First(f=>f.KeyValueIntCode == "LicensingCPO");
                var licensingCIPO = this.licensingType.First(f=>f.KeyValueIntCode == "LicensingCIPO");

                if (candidateProviderVM.IdTypeLicense == licensingCPO.IdKeyValue) 
                {
                    candidateProviderVM.IdTypeApplication = this.typeApplication.First(f => f.KeyValueIntCode == "FirstLicenzing").IdKeyValue; //this.DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "").Result.IdKeyValue;
                }
                else if (candidateProviderVM.IdTypeLicense == licensingCIPO.IdKeyValue)
                {
                    candidateProviderVM.IdTypeApplication = this.typeApplication.First(f => f.KeyValueIntCode == "FirstLicensingCIPO").IdKeyValue;  //this.DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicensingCIPO").Result.IdKeyValue;
                }



               
                
                
                candidateProviderVM.IdApplicationStatus = this.DataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "PreparationDocumentationLicensing").Result.IdKeyValue;


                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("PoviderBulstat", candidateProviderVM.PoviderBulstat) };



                var minutesValidToken = await this.DataSourceService.GetSettingByIntCodeAsync("MinutesForEmailValidation");

                candidateProviderVM.Token = this.CommonService.GetTokenWithParams(tokenContext, Int32.Parse(minutesValidToken.SettingValue));

                currentContext.ResultContextObject = candidateProviderVM;
                currentContext = await this.CandidateProviderService.CreateCandidateProvider(currentContext);

                if (!currentContext.HasErrorMessages)
                {
                    var result = await this.uploadService.UploadFileAsync<CandidateProvider>(
                        this.startRegistrationModel.file,
                        this.startRegistrationModel.FileName, "CandidateProvider", currentContext.ResultContextObject.IdCandidate_Provider);


                    ResultContext<CandidateProviderVM> resultContext = new ResultContext<CandidateProviderVM>();

                    resultContext.ResultContextObject = candidateProviderVM;


                    await this.MailService.SendEmailNewRegistration(resultContext);

                    await this.ShowSuccessAsync($"Вие подадохте успешно заявка за електронна регистрация в Информационната система на НАПОО! Моля, проверете Вашата пощенска кутия на адрес '{this.startRegistrationModel.Email}' и потвърдете регистрацията на посочения в e-mail-а линк.");


                    this.timer = new Timer(GlobalConstants.TIME_TO_REDIRECT_AFTER_REGISTRATION);
                    timer.Elapsed += RedirectToLogin;
                    timer.AutoReset = false;

                    timer.Start();

                }
                else
                {
                    await this.ShowErrorAsync("Неуспешна заявка.");
                }
                //this.isSubmitClicked = false;
            }

        }

        private void RedirectToLogin(Object source, ElapsedEventArgs e)
        {
            InvokeAsync(async () =>
            {
                timer.Stop();
                navMgr.NavigateTo("login", true);

            });
        }



        private async Task OnChange(UploadChangeEventArgs args)
        {

            this.startRegistrationModel.file = args.Files[0].Stream;
            this.startRegistrationModel.HasUploadedFile = true;
            this.startRegistrationModel.FileName = args.Files[0].FileInfo.Name;

            this.StateHasChanged();


        }

        private void ValidationRequested(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();



            if (useAttorney && string.IsNullOrEmpty(this.startRegistrationModel.AttorneyName))
            {
                FieldIdentifier fi = new FieldIdentifier(this.startRegistrationModel, "AttorneyName");
                this.messageStore?.Add(fi, "Полето 'Пълномощник ' е задължително");
            }

            if (useAttorney && string.IsNullOrEmpty(this.startRegistrationModel.FileName))
            {
                FieldIdentifier fi = new FieldIdentifier(this.startRegistrationModel, "FileName");
                this.messageStore?.Add(fi, "В случаите, в които електронната регистрация се прави от упълномощено лице, следва да прикачите файл със сканирано пълномощно!");
            }

            Regex regex = new Regex("(?:[a-z0-9!#$%&'*+\\=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+\\=?^_`{|}~-]+)*|\"\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\"\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
            Match match = regex.Match(this.startRegistrationModel.Email);
            if (!match.Success)
            {
                FieldIdentifier fi = new FieldIdentifier(this.startRegistrationModel, "Email");
                this.messageStore?.Add(fi, "Въведеният E-mail е невалиден!");
            }
        }
        private async void OnDownloadClick()
        {
            if (this.startRegistrationModel.HasUploadedFile)
            {
                await FileUtils.SaveAs(this.JsRuntime, this.startRegistrationModel.FileName, this.startRegistrationModel.file.ToArray());
            }
            else
            {
                await this.JsRuntime.InvokeVoidAsync("alert", "Файлът, който се опитвате да свалите, не съществува!");
            }
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (this.startRegistrationModel.HasUploadedFile)
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    this.startRegistrationModel.file = null;
                    this.startRegistrationModel.HasUploadedFile = false;
                    this.startRegistrationModel.FileName = String.Empty;



                    this.StateHasChanged();
                }
            }
        }
        private async Task OnRemove(RemovingEventArgs args)
        {
            //if (args.FilesData.Count == 1)
            //{
            //    if (this.model.IdOrder > 0)
            //    {
            //        bool isConfirmed = true;
            //        if (args.FilesData[0].Name == this.model.FileName)
            //        {
            //            isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
            //        }

            //        if (isConfirmed)
            //        {
            //            var result = await this.uploadService.RemoveFileAsync<SPPOOOrder>(this.model.IdOrder);

            //            if (result == 1)
            //            {
            //                this.model = await this.orderService.GetOrderByIdAsync(this.model.IdOrder);

            //                this.StateHasChanged();
            //            }
            //        }
            //    }
            //}
        }

        public override void SubmitHandler()
        {
            throw new NotImplementedException();
        }
    }

}
